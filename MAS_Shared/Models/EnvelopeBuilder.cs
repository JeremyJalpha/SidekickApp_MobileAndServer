using MAS_Shared.Data;
using MAS_Shared.MASConstants;

namespace MAS_Shared.Models
{
    public static class EnvelopeBuilder
    {
        public static ChatUpdate FromWebhook(string senderCell, string body, ChatChannelType channel) =>
            new ChatUpdate
            {
                From = new ApplicationUser { CellNumber = senderCell },
                Body = body,
                Channel = channel
            };
    }
}
