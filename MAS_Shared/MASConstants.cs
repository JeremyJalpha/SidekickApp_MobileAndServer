namespace MAS_Shared.MASConstants
{
    public enum JwtTokenType
    {
        Unknown,
        ShortLived,
        LongLived
    }

    public enum ChatChannelType
    {
        None,
        WhatsApp,
        Telegram
    }

    public static class TokenStorageKeys
    {
        public const string Unknown = "unknown_jwt";
        public const string ShortLived = "short_lived_jwt";
        public const string LongLived = "long_lived_jwt";

        public static string ForType(JwtTokenType type) => type switch
        {
            JwtTokenType.ShortLived => ShortLived,
            JwtTokenType.LongLived => LongLived,
            _ => Unknown
        };
    }

    public static class MASConstants
    {
        public const string LocationRequestGroup = "LocationRequest";
        public const string ToGPSHubSendLastKnownLocOfClient = "ToGPSHubSendLastKnownLocOfClient";
        public const string FromAuthHubGetLongLivedJWT = "FromAuthHubGetLongLivedJWT";
        public const string JoinLocationRequestGroup = "JoinLocationRequestGroup";
        public const string JoinLocationRequestGroupFailed = "JoinLocationRequestGroupFailed";
        public const string SendLastKnownLocOfClientFailed = "SendLastKnownLocOfClientFailed";
        public const string ExitLocationRequestGroup = "ExitLocationRequestGroup";
        public const string AuthHubUrl = "https://localhost:443/AuthenticationHub";
        public const string GPSHubUrl = "https://localhost:443/GPSLocationHub";
        public const string ExpectedIssuer = "https://localhost:443";
        public static readonly List<string> ExpectedAudiences = new List<string> { "https://localhost:443" };
        public const string MessageQueueHostName = "localhost";
        public const string CommandQueueName = "command_queue";
        public const string TelegramOutboundQueue = "telegram_outbound_queue";
        public const string WhatsAppOutboundQueue = "whatsapp_outbound_queue";
    }
}
