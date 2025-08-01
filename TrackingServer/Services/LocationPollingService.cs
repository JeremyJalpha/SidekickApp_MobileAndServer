using Microsoft.AspNetCore.SignalR;
using TrackingServer.SignalR;
using MAS_Shared.MASConstants;

namespace TrackingServer.Services
{
    public class LocationPollingService : IHostedService, IDisposable
    {
        private readonly IHubContext<GPSLocationHub> _hubContext;
        private Timer? _timer;

        public LocationPollingService(IHubContext<GPSLocationHub> hubContext)
        {
            _hubContext = hubContext;
            _timer = null;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(PollClients, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void PollClients(object? state)
        {
            await _hubContext.Clients.Group(MASConstants.LocationRequestGroup).SendAsync(MASConstants.ToGPSHubSendLastKnownLocOfClient);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
