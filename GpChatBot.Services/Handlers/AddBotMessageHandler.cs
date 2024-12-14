using GpChatBot.Database;
using GpChatBot.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GpChatBot.Services.Handlers;

public class AddBotMessageCommand : IRequest<BotMessage>
{
    public AddBotMessageCommand(string message, Guid chatId)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        ChatId = chatId;
    }

    public string Message { get; }
    public Guid ChatId { get; }
}

internal class AddBotMessageHandler : IRequestHandler<AddBotMessageCommand, BotMessage>
{
    private readonly ChatDbContext _context;

    public AddBotMessageHandler(ChatDbContext context)
    {
        _context = context;
    }
    
    public async Task<BotMessage> Handle(AddBotMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await _context.Chats
            .SingleOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken);
         
        if (chat == null)
        {
            throw new Exception($"Chat with id {request.ChatId} not found");
        }

        var newMessage = new BotMessage(Guid.NewGuid(), request.Message, DateTime.UtcNow, chat.Id);
        _context.BotMessages.Add(newMessage);
        await _context.SaveChangesAsync(cancellationToken);

        return newMessage;
    }
}