using System.ComponentModel.DataAnnotations;
using GpChatBot.Database.Model;

namespace GpChatBot.Database.Models;

public class BotMessage
{
    private BotMessage(){}

    public BotMessage(Guid id, string message, DateTime createdAt, Guid chatId)
    {
        Id = id;
        Message = message;
        CreatedAt = createdAt;
        ChatId = chatId;
    }
    
    public Guid Id { get; set; }
    [MaxLength(1000)]
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? IsLiked { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
}