using CommandBot.Services;
using MAS_Shared.MASConstants;
using MAS_Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("webhook/telegram")]
public class TelegramController : ControllerBase
{
    private readonly ILogger<TelegramController> _logger;
    private readonly RabbitMQService _rabbit;

    public TelegramController(ILogger<TelegramController> logger, RabbitMQService rabbit)
    {
        _logger = logger;
        _rabbit = rabbit;
    }

    [HttpPost]
    public Task<IActionResult> ReceiveTelegramUpdate([FromBody] Telegram.Bot.Types.Update update)
    {
        if (update is null || update.Message is null || update.ChatMember is null)
        {
            _logger.LogWarning("Received null or invalid Telegram update.");
            return Task.FromResult<IActionResult>(BadRequest());
        }
        var senderId = update.ChatMember.Chat.Id.ToString();
        var messageText = update.Message.Text;

        if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(messageText))
        {
            _logger.LogWarning("Telegram webhook received invalid payload.");
            return Task.FromResult<IActionResult>(BadRequest());
        }

        var payload = EnvelopeBuilder.FromWebhook(senderId, messageText, ChatChannelType.Telegram);

        var json = JsonSerializer.Serialize(payload);
        _rabbit.PublishCommand(json);
        return Task.FromResult<IActionResult>(Ok());
    }
}