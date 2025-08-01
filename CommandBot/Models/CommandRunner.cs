using CommandBot.Interfaces;
using CommandBot.Services;
using MAS_Shared.Data;
using MAS_Shared.MASConstants;
using MAS_Shared.Models;
using Microsoft.Extensions.Options;

namespace CommandBot.Models
{
    public class CommandRunner : ICommandRunner
    {
        private readonly AppDbContext _db;
        private readonly BusinessContext _busi;
        private readonly JwtIssueConfig _jwtConfig;
        private readonly ConversationContextFactory _convoFactory;
        private readonly ILogger<CommandRunner> _logger;

        public CommandRunner(
            AppDbContext db,
            BusinessContext busi,
            IOptions<JwtIssueConfig> jwtConfig,
            ConversationContextFactory convoFactory,
            ILogger<CommandRunner> logger)
        {
            _db = db;
            _busi = busi;
            _jwtConfig = jwtConfig.Value;
            _convoFactory = convoFactory;
            _logger = logger;
        }

        public async Task<List<ChatDispatchRequest>> ExecuteAsync(ChatUpdate chatUpdate)
        {
            try
            {
                var convo = _convoFactory.CreateConversationContext(_db, chatUpdate);
                var ctx = new CommandContext(_db, convo, _busi, _jwtConfig, _logger);
                var commands = CommandParser.ParseCommands(convo);
                var processor = new CommandProcessor(commands);

                return await processor.ProcessCommandsAsync(ctx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CommandRunner execution failed for {From}", chatUpdate.From);

                return new List<ChatDispatchRequest>
                {
                    new ChatDispatchRequest
                    {
                        ChatUpdate = new ChatUpdate
                        {
                            From = chatUpdate.From,
                            Channel = ChatChannelType.None,
                            Body = "An unexpected error occurred while processing your request.",
                        },
                        Tags = new Dictionary<string, string> { { "error", "true" } }
                    }
                };
            }
        }
    }
}
