using MAS_Shared.Data;
using MAS_Shared.Models;

namespace CommandBot.Models
{
    public sealed class CommandContext
    {
        public ConversationContext ConvoContext { get; }
        public BusinessContext BusiContext { get; }

        public AppDbContext AppDbContext { get; init; } = null!;

        public JwtIssueConfig JwtConfig { get; init; } = null!;

        public ILogger Logger { get; }

        public CommandContext(AppDbContext appdbcontext, ConversationContext convo, BusinessContext busi, JwtIssueConfig jwtconfig, ILogger logger)
        {
            ConvoContext = convo ?? throw new ArgumentNullException(nameof(convo));
            BusiContext = busi ?? throw new ArgumentNullException(nameof(busi));
            AppDbContext = appdbcontext ?? throw new ArgumentNullException(nameof(appdbcontext));
            JwtConfig = jwtconfig ?? throw new ArgumentNullException(nameof(jwtconfig));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
