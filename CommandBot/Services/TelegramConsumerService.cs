using MAS_Shared.Interfaces.CommandBot.Interfaces;
using MAS_Shared.MASConstants;
using MAS_Shared.Models;
using System.Text.Json;

namespace CommandBot.Services
{
    public class TelegramConsumerService : BackgroundService
    {
        private readonly ILogger<TelegramConsumerService> _logger;
        private readonly RabbitMQService _rabbit;
        private readonly ITelegramDispatchService _dispatch;

        public TelegramConsumerService(
            ILogger<TelegramConsumerService> logger,
            RabbitMQService rabbit,
            ITelegramDispatchService dispatch)
        {
            _logger = logger;
            _rabbit = rabbit;
            _dispatch = dispatch;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbit.StartConsumingTelegram(async json => await HandleTelegramMessage(json));

            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }

        private async Task HandleTelegramMessage(string json)
        {
            var payload = JsonSerializer.Deserialize<ChatDispatchRequest>(json);
            if (!IsValidTelegramPayload(payload))
                return;

            if (payload is null || payload.ChatUpdate.From.CellNumber is null)
            {
                _logger.LogWarning("Telegram message is null or missing sender information.");
                return;
            }

            await _dispatch.DispatchAsync(payload);
        }

        private static bool IsValidTelegramPayload(ChatDispatchRequest? payload)
            => payload?.ChatUpdate?.Channel == ChatChannelType.Telegram
               && !string.IsNullOrWhiteSpace(payload.ChatUpdate.Body);
    }
}
