using MAS_Shared.Data;
using MAS_Shared.MASConstants;
using MAS_Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TrackingServer.Interfaces;

namespace TrackingServer.SignalR
{
    [Authorize(Roles = "Driver")]
    public class GPSLocationHub : Hub
    {
        private readonly ICoordinateService _coordinateService;
        private readonly AppDbContext _dbContext;
        private readonly CoordUpdateFactory _updateFactory;

        public GPSLocationHub(ICoordinateService coordinateService, AppDbContext dbcontext)
        {
            _coordinateService = coordinateService;
            _dbContext = dbcontext;
            _updateFactory = new CoordUpdateFactory(_dbContext);
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var role = user.FindFirst(ClaimTypes.Role)?.Value;
                Console.WriteLine($"✅ User connected with role: {role}");
            }
            else
            {
                Console.WriteLine("❌ User not authenticated.");
            }

            await base.OnConnectedAsync();
        }

        public async Task<Exception?> JoinLocationRequestGroup(GPSLocationDTO coords)
        {
            if (Context.User == null)
                return new Exception("User context is null.");

            if (!_updateFactory.TryBuild(Context.User, coords, out var update, out var error))
                return error;

            if (update == null)
                return new Exception("Failed to build CoordUpdateModel from user context and coordinates.");

            await Groups.AddToGroupAsync(Context.ConnectionId, MASConstants.LocationRequestGroup);
            return await _coordinateService.HandleCoordinates(update);
        }

        public async Task<Exception?> ExitLocationRequestGroup(GPSLocationDTO coords)
        {
            if (Context.User == null)
                return new Exception("User context is null.");

            if (!_updateFactory.TryBuild(Context.User, coords, out var update, out var error))
                return error;

            if (update == null)
                return new Exception("Failed to build CoordUpdateModel from user context and coordinates.");

            var handleCoordResult = await _coordinateService.HandleCoordinates(update);
            if (handleCoordResult != null)
                return handleCoordResult;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, MASConstants.LocationRequestGroup);
            return null;
        }

        public async Task<Exception?> ToGPSHubSendLastKnownLocOfClient(GPSLocationDTO coords)
        {
            if (Context.User == null)
                return new Exception("User context is null.");

            if (!_updateFactory.TryBuild(Context.User, coords, out var update, out var error))
                return error;

            if (update == null)
                return new Exception("Failed to build CoordUpdateModel from user context and coordinates.");

            // Add to group if not already (idempotent in SignalR)
            await Groups.AddToGroupAsync(Context.ConnectionId, MASConstants.LocationRequestGroup);

            // Update the coordinates in the DB
            var handleCoordResult = await _coordinateService.HandleCoordinates(update);
            if (handleCoordResult != null)
                return handleCoordResult;

            return null;
        }
    }
}
