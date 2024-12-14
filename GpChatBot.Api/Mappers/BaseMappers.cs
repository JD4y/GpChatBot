using System.Diagnostics;
using GpChatBot.ApiModels.Response;
using GpChatBot.Database.Model;
using GpChatBot.Database.Models;

namespace GpChatBot.Mappers;

public static class BaseMappers
{
    public static MessageApiModel Map(this BotMessage message)
    {
        return new MessageApiModel(
            message.Id, 
            message.ChatId,
            message.Message, 
            message.CreatedAt,
            message.IsLiked,
            true);
    } 
    
    public static MessageApiModel Map(this UserMessage message)
    {
        return new MessageApiModel(
            message.Id, 
            message.ChatId,
            message.Message, 
            message.CreatedAt,
            null,
            false);
    }

    public static ChatApiModel Map(this Chat chat)
    {
        var botMessages = chat.BotMessages
            .Select(x => x.Map())
            .ToList();

        var userMessages = chat.UserMessages
            .Select(x => x.Map())
            .ToList();
        
        var allMessagesSorted = botMessages.Concat(userMessages)
            .OrderBy(x => x.CreatedAt)
            .ToList();
        
        return new ChatApiModel(chat.Id, chat.StartedAt, allMessagesSorted);
    }
    
}