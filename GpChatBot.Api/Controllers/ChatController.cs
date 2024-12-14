using GpChatBot.ApiModels.Request;
using GpChatBot.ApiModels.Response;
using GpChatBot.Mappers;
using GpChatBot.Services.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GpChatBot.Controllers;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageApiModel))]
    public async Task<IActionResult> UpdateBotMessageStatus([FromBody] UpdateBotMessageStatusRequest request)
    {
        var result = await _mediator.Send(new LikeBotMessageCommand(request.ChatId, request.MessageId, request.Like));
        
        return Ok(result.Map());
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChatApiModel>))]
    public async Task<IActionResult> GetChats()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(ipAddress))
        {
            throw new InvalidOperationException("IP address is missing.");
        }
        
        var result = await _mediator.Send(new GetChatHistoryQuery(ipAddress));
        
        return Ok(result.Select(x => x.Map()).ToList());
    }
}