using GpChatBot.Database;
using GpChatBot.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GpChatBot.Services.Handlers;

public class UpdateBotMessageCommand : IRequest<BotMessage>
{
    public UpdateBotMessageCommand(Guid messageId, Guid chatId, string message)
    {
        MessageId = messageId;
        ChatId = chatId;
        Message = message;
    }

    public Guid MessageId { get; }
    public Guid ChatId { get; }
    public string Message { get; }
}

internal class UpdateBotMessageHandler : IRequestHandler<UpdateBotMessageCommand, BotMessage>
{
    private readonly ChatDbContext _context;

    public UpdateBotMessageHandler(ChatDbContext context)
    {
        _context = context;
    }
    
    public async Task<BotMessage> Handle(UpdateBotMessageCommand request, CancellationToken cancellationToken)
    {
        var chatEntity = await _context.Chats
            .Include(x => x.BotMessages)
            .SingleOrDefaultAsync(x => x.Id == request.ChatId, cancellationToken);

        if (chatEntity == null)
        {
            throw new ApplicationException($"Chat with id {request.ChatId} not found");
        }

        var messageEntity = chatEntity.BotMessages
            .SingleOrDefault(x => x.Id == request.MessageId);

        if (messageEntity == null)
        {
            throw new ApplicationException($"Message with id {request.MessageId} not found");
        }
        var result = string.Concat(messageEntity.Message, " ", request.Message); 
        messageEntity.Message = result;
        await _context.SaveChangesAsync(cancellationToken);
        
        return messageEntity;    
    }
}