using Microsoft.AspNetCore.SignalR.Client;

namespace SidekickApp_MAUIHybridMobile.SignalR
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;

        public async Task StartConnectionAsync(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            await _hubConnection.StartAsync();
        }

        public async Task SendMessageAsync(string user, string message)
        {
            if (_hubConnection == null)
            {
                throw new InvalidOperationException("Connection has not been started.");
            }
            await _hubConnection.InvokeAsync("SendMessage", user, message);
        }

        public void RegisterReceiveMessageHandler(Action<string, string> handler)
        {
            if (_hubConnection == null)
            {
                throw new InvalidOperationException("Connection has not been started.");
            }
            _hubConnection.On("ReceiveMessage", handler);
        }
    }
}