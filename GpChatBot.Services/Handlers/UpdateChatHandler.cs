using GpChatBot.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GpChatBot.Services.Handlers;

public class UpdateChatCommand : IRequest
{
    public UpdateChatCommand(Guid chatId)
    {
        ChatId = chatId;
    }
    
    public Guid ChatId { get; }
}

public class UpdateChatHandler: IRequestHandler<UpdateChatCommand>
{
    private readonly ChatDbContext _context;

    public UpdateChatHandler(ChatDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(UpdateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _context.Chats
            .SingleOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken);
         
        if (chat == null)
        {
            throw new Exception($"Chat with id {request.ChatId} not found");
        }    
        chat.EndedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}