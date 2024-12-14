namespace GpChatBot.ApiModels.Request;

public class UpdateBotMessageStatusRequest
{
    public Guid ChatId { get; set; }
    public Guid MessageId { get; set; }
    public bool Like { get; set; }
}