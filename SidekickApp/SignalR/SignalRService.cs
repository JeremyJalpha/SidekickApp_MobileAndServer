using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using MAS_Shared.Models;
using MAS_Shared.MASConstants;

namespace SidekickApp.SignalR
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;
        private readonly ILogger<SignalRService> _logger;

        public event Action<string>? OnSendLastKnownLocOfClientFailed;
        public event Action<bool>? OnConnectionChanged;
        public event Action<bool>? OnLocationBroadcastingChanged;

        public bool IsConnected =>
            _hubConnection != null && _hubConnection.State == HubConnectionState.Connected;

        public SignalRService(ILogger<SignalRService> logger)
        {
            _logger = logger;
        }

        private void RaiseConnectionChanged(bool isConnected)
        {
            OnConnectionChanged?.Invoke(isConnected);
        }

        private void RaiseBroadcastingChanged(bool isBroadcasting)
        {
            OnLocationBroadcastingChanged?.Invoke(isBroadcasting);
        }

        public void RegisterHandlers()
        {
            _hubConnection?.On(MASConstants.ToGPSHubSendLastKnownLocOfClient, async () =>
            {
                var sendLocResult = await ToGPSHubSendLastKnownLocOfClient();
                if (sendLocResult != null)
                    OnSendLastKnownLocOfClientFailed?.Invoke(sendLocResult.Message);
            });

            _hubConnection?.On<string>(MASConstants.JoinLocationRequestGroupFailed, errorMsg =>
            {
                OnSendLastKnownLocOfClientFailed?.Invoke(errorMsg);
            });

            _hubConnection?.On<string>(MASConstants.SendLastKnownLocOfClientFailed, errorMsg =>
            {
                OnSendLastKnownLocOfClientFailed?.Invoke(errorMsg);
            });
        }

        public async Task StartConnWithJWTAsync(string hubUrl, string jwt)
        {
            await StopConnectionAsync();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(jwt);
                })
                .Build();

            try
            {
                await _hubConnection.StartAsync();
                _logger.LogInformation("✅ SignalR connection established with {HubUrl}", hubUrl);
                RegisterHandlers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while starting SignalR connection to {HubUrl}", hubUrl);
            }

            RaiseConnectionChanged(true);
        }

        public async Task StopConnectionAsync()
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _logger.LogInformation("🔌 SignalR connection stopped.");
                _hubConnection = null;
            }

            RaiseConnectionChanged(false);
        }

        public async Task<Exception?> ToGPSHubSendLastKnownLocOfClient()
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
                throw new InvalidOperationException("SignalR ➜ Connection not active.");

            var gpsCoords = await GPSLocationService.GetCurrentLocationAsync();
            return await _hubConnection.InvokeAsync<Exception?>(MASConstants.ToGPSHubSendLastKnownLocOfClient, gpsCoords);
        }

        public async Task<Exception?> JoinLocationRequestGroup(GPSLocationDTO gpsCoords)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
                throw new InvalidOperationException("SignalR ➜ Connection not active.");

            var result = await _hubConnection.InvokeAsync<Exception?>(MASConstants.JoinLocationRequestGroup, gpsCoords);

            if (result == null)
            {
                RaiseBroadcastingChanged(true);
                _logger.LogInformation("📍 Joined location request group.");
            }

            return result;
        }

        public async Task<Exception?> ExitLocationRequestGroup(GPSLocationDTO gpsCoords)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
                throw new InvalidOperationException("SignalR ➜ Connection not active.");

            var result = await _hubConnection.InvokeAsync<Exception?>(MASConstants.ExitLocationRequestGroup, gpsCoords);

            if (result == null)
            {
                RaiseBroadcastingChanged(false);
                _logger.LogInformation("📴 Exited location request group.");
            }

            return result;
        }

        public async Task<(string?, Exception?)> FromAuthHubGetLongLivedJWT(string shortLivedToken)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
                throw new InvalidOperationException("SignalR ➜ Connection not active.");

            return await _hubConnection.InvokeAsync<(string?, Exception?)>(MASConstants.FromAuthHubGetLongLivedJWT, shortLivedToken);
        }
    }
}