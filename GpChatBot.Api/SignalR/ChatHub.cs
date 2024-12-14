using System.Collections.Concurrent;
using GpChatBot.ApiModels.Response;
using GpChatBot.Mappers;
using GpChatBot.Services.Handlers;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace GpChatBot.SignalR;

public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    
    private static readonly ConcurrentDictionary<string, Guid> ActiveClients = new();
    private static readonly Random Random = new();

    private const string WelcomeMessage =
        "Hello! I am your friendly chatbot assistant. How can I assist you today?";

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override async Task OnConnectedAsync()
    {
        var ipAddress = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(ipAddress))
        {
            throw new HubException("No connection could be made.");
        }
        
        var chatId = await _mediator.Send(new CreatChatCommand(ipAddress));
        ActiveClients[Context.ConnectionId] = chatId;
        var message = await _mediator.Send(new AddBotMessageCommand(WelcomeMessage, chatId));
        await Clients.Caller.SendAsync("ReceiveMessage", message.Map());

        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        ActiveClients.TryRemove(Context.ConnectionId, out var chatId);
        await _mediator.Send(new UpdateChatCommand(chatId));
        
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendMessage(string message)
    {
        if (ActiveClients.TryGetValue(Context.ConnectionId, out var chatId))
        {
            var userMessage = await _mediator.Send(new AddUserMessageCommand(chatId, message));
            await Clients.Caller.SendAsync("ReceiveMessage", userMessage.Map());

            await SendLoremIpsum(Random.Next(1, 6), chatId);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Unable to send message. Please reconnect.");
        }
    }
    
    private async Task SendLoremIpsum(int sentenceCount, Guid chatId)
    {
        if (sentenceCount < 1 || sentenceCount > 10)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Error", "Please provide a value between 1 and 5.");
            return;
        }

        string[] loremSentences = {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            "Curabitur pretium tincidunt lacus. Nulla gravida orci a odio. Nullam varius, turpis et commodo pharetra, est eros bibendum elit, nec luctus magna felis sollicitudin mauris.",
            "Integer vitae libero ac risus egestas placerat. Nulla facilisi. Aenean sit amet justo. Praesent ullamcorper velit vel facilisis volutpat.",
            "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante.",
            "Fusce condimentum nunc ac nisi vulputate fringilla. Donec lacinia congue felis in faucibus.",
            "Nulla posuere. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante."
        };


        Guid? generateBotMessageId = null;
        for (var i = 0; i < sentenceCount; i++)
        {
            var text = loremSentences[i];
    
            var words = text.Split(' ');
    
            foreach (var word in words)
            {
                MessageApiModel message;
                if (i == 0 && words.First() == word) 
                {
                    var newBotMessage = await _mediator.Send(new AddBotMessageCommand(word, chatId));
                    generateBotMessageId = newBotMessage.Id;
                    message = newBotMessage.Map();
                }
                else
                {
                    if (!generateBotMessageId.HasValue)
                    {
                        throw new ApplicationException("Bot message Id cannot be null");
                    }
            
                    var updatedText = text.Replace(words.First(), word);
                    var updateBotMessage = await _mediator.Send(new UpdateBotMessageCommand(generateBotMessageId.Value, chatId, updatedText));
                    message = updateBotMessage.Map();
                }

                await Clients.Caller.SendAsync("ReceiveMessage", message);
                await Task.Delay(800);
            }
        }

    }
}