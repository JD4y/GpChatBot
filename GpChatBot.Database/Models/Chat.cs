using System.ComponentModel.DataAnnotations;
using GpChatBot.Database.Models;

namespace GpChatBot.Database.Model;

public class Chat
{
    private Chat()
    {
    }

    public Chat(Guid id, string clientIpAddress, DateTime startedAt)
    {
        Id = id;
        ClientIpAddress = clientIpAddress;
        StartedAt = startedAt;
    }
    
    public Guid Id { get; set; }
    [MaxLength(50)]
    public string ClientIpAddress { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public List<UserMessage> UserMessages { get; set; } = [];
    public List<BotMessage> BotMessages { get; set; } = [];
}