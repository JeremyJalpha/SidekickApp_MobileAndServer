using MAS_Shared.Interfaces.CommandBot.Interfaces;
using MAS_Shared.Models;
using Telegram.Bot;

namespace CommandBot.Services
{
    public class TelegramDispatchService : ITelegramDispatchService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TelegramDispatchService> _logger;

        public TelegramDispatchService(ITelegramBotClient botClient, ILogger<TelegramDispatchService> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task DispatchAsync(ChatDispatchRequest dispatch)
        {
            var chatId = long.Parse(dispatch.ChatUpdate.From.CellNumber);
            var text = dispatch.ChatUpdate.Body;

            try
            {
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: text,
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send Telegram message to {ChatId}", chatId);
            }
        }
    }
}
