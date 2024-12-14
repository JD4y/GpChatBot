using System.ComponentModel.DataAnnotations;

namespace GpChatBot.Database.Model;

public class UserMessage
{
    private UserMessage() {}

    public UserMessage(Guid id, string message, DateTime createdAt, Guid chatId)
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
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
}
