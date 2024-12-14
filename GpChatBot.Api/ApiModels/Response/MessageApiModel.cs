namespace GpChatBot.ApiModels.Response;

public class MessageApiModel
{
    public MessageApiModel(Guid id, Guid chatId, string message, DateTime createdAt, bool? isLiked, bool isBot)
    {
        Id = id;
        ChatId = chatId;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        CreatedAt = createdAt;
        IsLiked = isLiked;
        IsBot = isBot;
    }

    public Guid Id { get; }
    public Guid ChatId { get; }
    public string Message { get; }
    public DateTime CreatedAt { get; }
    public bool? IsLiked { get; }
    public bool IsBot { get; }
}