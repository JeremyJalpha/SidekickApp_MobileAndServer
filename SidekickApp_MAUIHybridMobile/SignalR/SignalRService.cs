using Microsoft.AspNetCore.SignalR.Client;

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

    public async Task SendJwtAsync(string jwt)
    {
        if (_hubConnection == null)
        {
            throw new InvalidOperationException("Connection has not been started.");
        }
        await _hubConnection.InvokeAsync("SendJwtToServer", jwt);
    }

    // Register the ReceiveLoginResultAsync handler
    public void RegisterReceiveLoginResultHandler(Action<string> handler)
    {
        if (_hubConnection == null)
        {
            throw new InvalidOperationException("Connection has not been started.");
        }
        _hubConnection.On("ReceiveLoginResultAsync", handler);
    }
}
