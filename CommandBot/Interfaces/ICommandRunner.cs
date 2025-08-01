using MAS_Shared.Data;
using MAS_Shared.Models;

namespace CommandBot.Interfaces
{
    public interface ICommandRunner
    {
        Task<List<ChatDispatchRequest>> ExecuteAsync(ChatUpdate chatUpdate);
    }
}
