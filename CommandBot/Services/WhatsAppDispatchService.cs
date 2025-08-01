using MAS_Shared.Interfaces.CommandBot.Interfaces;
using MAS_Shared.Models;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace CommandBot.Services
{
    public class WhatsAppDispatchService : IWhatsAppDispatchService
    {
        private readonly IWhatsAppBusinessClient _whatsAppClient;
        private readonly ILogger<WhatsAppDispatchService> _logger;

        public WhatsAppDispatchService(
            IWhatsAppBusinessClient whatsAppClient,
            ILogger<WhatsAppDispatchService> logger)
        {
            _whatsAppClient = whatsAppClient;
            _logger = logger;
        }

        public async Task DispatchAsync(ChatDispatchRequest dispatch)
        {
            var userId = dispatch.ChatUpdate.From.Id;

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Skipping WhatsApp dispatch due to missing UserID.");
                return;
            }

            var message = dispatch.ChatUpdate.Body;

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Skipping WhatsApp dispatch due to missing message content.");
                return;
            }

            await _whatsAppClient.SendTextMessageAsync(new TextMessageRequest
            {
                To = userId,
                Text = new WhatsAppText
                {
                    Body = message,
                    PreviewUrl = false // optional, set to true if you want link previews
                }
            });
        }
    }
}
