using CommandBot.Models;

namespace CommandBot.Interfaces
{
    public interface ICommand
    {
        Task<string> ExecuteAsync(CommandContext context);
    }
}
