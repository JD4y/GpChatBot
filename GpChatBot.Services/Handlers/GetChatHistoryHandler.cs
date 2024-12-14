using GpChatBot.Database;
using GpChatBot.Database.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GpChatBot.Services.Handlers;

public class GetChatHistoryQuery : IRequest<IReadOnlyList<Chat>>
{
    public GetChatHistoryQuery(string ipAddress)
    {
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
    }

    public string IpAddress { get; }
}

internal class GetChatHistoryHandler : IRequestHandler<GetChatHistoryQuery, IReadOnlyList<Chat>>
{
    private readonly ChatDbContext _context;

    public GetChatHistoryHandler(ChatDbContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyList<Chat>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var chats = await _context.Chats
            .Include(x => x.UserMessages)
            .Include(x => x.BotMessages)
            .Where(x => x.ClientIpAddress == request.IpAddress)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(cancellationToken);

        return chats;
    }
}