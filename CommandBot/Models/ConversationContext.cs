using MAS_Shared.Data;
using MAS_Shared.MASConstants;

namespace CommandBot.Models
{
    public class ConversationContext
    {
        public string MsgOriginNumber { get; private set; }
        public string MessageBody { get; private set; }
        public bool UserExisted { get; set; } = false;
        public ApplicationUser User { get; set; }
        public string? CurrentOrder { get; set; }

        public ChatChannelType Channel { get; init; }

        public ConversationContext(ApplicationUser user, bool userexisted, string msgOriginNumber, string messageBody, ChatChannelType channel)
        {
            User = user ?? throw new ArgumentException("User cannot be null or empty.", nameof(user));

            UserExisted = userexisted;

            MsgOriginNumber = msgOriginNumber ?? throw new ArgumentException("Message origin number cannot be null or empty.", nameof(msgOriginNumber));

            MessageBody = messageBody ?? throw new ArgumentException("Message body cannot be null or empty.", nameof(messageBody));

            Channel = channel;
        }
    }
}