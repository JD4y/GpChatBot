namespace GpChatBot.ApiModels.Response;

public class ChatApiModel
{
    public ChatApiModel(Guid id, DateTime startedAt, IReadOnlyList<MessageApiModel> messages)
    {
        Id = id;
        StartedAt = startedAt;
        Messages = messages;
    }

    public Guid Id { get; }
    public DateTime StartedAt { get; set; }
    public IReadOnlyList<MessageApiModel> Messages { get; }
}