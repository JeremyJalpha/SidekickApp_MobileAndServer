using CommandBot.Interfaces;
using MAS_Shared.Models;

namespace CommandBot.Models
{
    public class CommandProcessor
    {
        private readonly List<ICommand> _commands;

        public CommandProcessor(List<ICommand> commands)
        {
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public async Task<List<ChatDispatchRequest>> ProcessCommandsAsync(CommandContext ctx)
        {
            var dispatches = new List<ChatDispatchRequest>();

            if (_commands.Count == 0)
            {
                dispatches.Add(new ChatDispatchRequest
                {
                    ChatUpdate = new ChatUpdate
                    {
                        From = ctx.ConvoContext.User,
                        Body = "No commands to process.",
                        Channel = ctx.ConvoContext.Channel
                    },
                    Tags = new Dictionary<string, string>
                    {
                        { "status", "noop" }
                    }
            });
                return dispatches;
            }

            foreach (var command in _commands)
            {
                var commandName = command.GetType().Name.Replace("Command", "").ToLowerInvariant();

                try
                {
                    var result = await command.ExecuteAsync(ctx);

                    dispatches.Add(new ChatDispatchRequest
                    {
                        ChatUpdate = new ChatUpdate
                        {
                            From = ctx.ConvoContext.User,
                            Body = result,
                            Channel = ctx.ConvoContext.Channel
                        },
                        Tags = new Dictionary<string, string>
                        {
                            { "command", commandName },
                            { "status", "ok" }
                        }
                    });
                }
                catch (Exception ex)
                {
                    ctx.Logger.LogError(ex, "Failed to execute command {CommandName}", commandName);

                    dispatches.Add(new ChatDispatchRequest
                    {
                        ChatUpdate = new ChatUpdate
                        {
                            From = ctx.ConvoContext.User,
                            Body = $"❌ Failed to execute `{commandName}`",
                            Channel = ctx.ConvoContext.Channel
                        },
                        Tags = new Dictionary<string, string>
                        {
                            { "command", commandName },
                            { "status", "error" },
                            { "exception", ex.GetType().Name }
                        }
                    });
                }
            }

            return dispatches;
        }
    }
}
