using MAS_Shared.Models;

namespace MAS_Shared.Interfaces
{
    namespace CommandBot.Interfaces
    {
        public interface IChatMessageDispatchService
        {
            Task DispatchAsync(ChatDispatchRequest dispatch);
        }

        public interface ITelegramDispatchService : IChatMessageDispatchService { }
        public interface IWhatsAppDispatchService : IChatMessageDispatchService { }
    }
}
