using GpChatBot.Database;
using GpChatBot.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GpChatBot.Services.Handlers;

public class LikeBotMessageCommand : IRequest<BotMessage>
{
    public LikeBotMessageCommand(Guid chatId, Guid messageId, bool like)
    {
        ChatId = chatId;
        MessageId = messageId;
        Like = like;
    }
    
    public Guid ChatId { get; }
    public Guid MessageId { get; }
    public bool Like { get; }
}

internal class LikeBotMessageHandler : IRequestHandler<LikeBotMessageCommand, BotMessage>
{
    private readonly ChatDbContext _context;

    public LikeBotMessageHandler(ChatDbContext context)
    {
        _context = context;
    }
    
    public async Task<BotMessage> Handle(LikeBotMessageCommand request, CancellationToken cancellationToken)
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
        
        messageEntity.IsLiked = request.Like;
        await _context.SaveChangesAsync(cancellationToken);
        
        return messageEntity;
    }
}