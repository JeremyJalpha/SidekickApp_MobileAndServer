using CommandBot.Interfaces;
using CommandBot.Services;
using MAS_Shared.MASConstants;
using System.Text.Json;

namespace CommandBot.Workers
{
    public class CommandWorker : BackgroundService
    {
        private readonly ILogger<CommandWorker> _logger;
        private readonly RabbitMQService _rabbit;
        private readonly ICommandRunner _runner;

        public CommandWorker(
            ILogger<CommandWorker> logger,
            RabbitMQService rabbit,
            ICommandRunner runner)
        {
            _logger = logger;
            _rabbit = rabbit;
            _runner = runner;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _rabbit.StartConsuming(ProcessMessage);

            while (!token.IsCancellationRequested)
                await Task.Delay(1000, token);
        }

        private void ProcessMessage(string json)
        {
            var payload = JsonSerializer.Deserialize<ChatUpdate>(json);

            if (payload is null || payload.From == null || string.IsNullOrWhiteSpace(payload.From.Id.ToString()) || string.IsNullOrWhiteSpace(payload.Body))
            {
                _logger.LogError("Bad payload: {json}", json);
                return;
            }

            _ = Task.Run(async () =>
            {
                var dispatches = await _runner.ExecuteAsync(payload);

                foreach (var dispatch in dispatches)
                {
                    var outboundJson = JsonSerializer.Serialize(dispatch);

                    switch (payload.Channel)
                    {
                        case ChatChannelType.Telegram:
                            _rabbit.PublishTelegramOutbound(outboundJson);
                            break;
                        case ChatChannelType.WhatsApp:
                            _rabbit.PublishWhatsAppOutbound(outboundJson);
                            break;
                        default:
                            _logger.LogWarning("Unknown channel: {Channel}", payload.Channel);
                            break;
                    }
                }
            });
        }
    }
}