using Microsoft.Extensions.Logging;
using SidekickApp.SignalR;

namespace SidekickApp.Services
{
    public class AppStateTracker
    {
        public string? LastVisitedRoute { get; set; }

        public string? DeferredToastMessage { get; set; }
        public LogLevel? DeferredToastType { get; set; } = LogLevel.Information;

        public bool IsConnectedToServer { get; set; } = false;

        public bool IsSharingLocation { get; set; } = false;

        public event Action<bool>? OnUiHydrated;

        public bool UiHydrated { get; private set; }

        public void RaiseUiHydrated(bool success)
        {
            UiHydrated = success;
            OnUiHydrated?.Invoke(success);
        }

        public void ConnectSignals(SignalRService signalR)
        {
            signalR.OnConnectionChanged += (isConnected) =>
            {
                IsConnectedToServer = isConnected;
            };

            signalR.OnLocationBroadcastingChanged += (isBroadcasting) =>
            {
                IsSharingLocation = isBroadcasting;
            };
        }

        public void SetDeferredToast(LogLevel lvl, string message)
        {
            DeferredToastType = lvl;
            DeferredToastMessage = message;
        }

        public Task InitializeAsync() => Task.CompletedTask;
    }
}