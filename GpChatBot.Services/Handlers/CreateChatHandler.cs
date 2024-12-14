using GpChatBot.Database;
using GpChatBot.Database.Model;
using MediatR;

namespace GpChatBot.Services.Handlers;

public class CreatChatCommand : IRequest<Guid>
{
    public CreatChatCommand(string ipAddress)
    {
        IpAddress = ipAddress;
    }
    
    public string IpAddress { get; }
}


internal class CreateChatHandler : IRequestHandler<CreatChatCommand, Guid>
{
    private readonly ChatDbContext _context;

    public CreateChatHandler(ChatDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatChatCommand request, CancellationToken cancellationToken)
    {
        var newChat = new Chat(Guid.NewGuid(), request.IpAddress, DateTime.UtcNow);
        
        _context.Chats.Add(newChat);
        await _context.SaveChangesAsync(cancellationToken);

        return newChat.Id;
    }
}