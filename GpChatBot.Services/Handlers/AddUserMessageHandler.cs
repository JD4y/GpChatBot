using GpChatBot.Database;
using GpChatBot.Database.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GpChatBot.Services.Handlers;

public class AddUserMessageCommand : IRequest<UserMessage>
{
    public AddUserMessageCommand(Guid chatId, string message)
    {
        ChatId = chatId;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public Guid ChatId { get; }
    public string Message { get; }
}

internal class AddUserMessageHandler : IRequestHandler<AddUserMessageCommand, UserMessage>
{
    private readonly ChatDbContext _context;

    public AddUserMessageHandler(ChatDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserMessage> Handle(AddUserMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await _context.Chats
            .SingleOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken);
         
        if (chat == null)
        {
            throw new Exception($"Chat with id {request.ChatId} not found");
        }

        var newMessage = new UserMessage(Guid.NewGuid(), request.Message, DateTime.UtcNow, chat.Id);
        _context.UserMessages.Add(newMessage);
        await _context.SaveChangesAsync(cancellationToken);

        return newMessage;
    }
}