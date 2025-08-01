using MAS_Shared.Data;
using MAS_Shared.Models;
using System.Security.Claims;
using TrackingServer.Models;

public class CoordUpdateFactory
{
    private readonly AppDbContext _db;

    public CoordUpdateFactory(AppDbContext db) => _db = db;

    public bool TryBuild(
        ClaimsPrincipal user, 
        GPSLocationDTO coords, 
        out CoordUpdateModel? update, 
        out Exception? error)
    {
        update = null;
        error = null;

        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            error = new Exception("User ID not found.");
            return false;
        }

        var driver = _db.Drivers.Find(userId);
        if (driver == null)
        {
            error = new Exception("Driver not found.");
            return false;
        }

        var order = _db.Orders
            .Where(o => o.DttmClosed == null && o.DisputedReason == null)
            .FirstOrDefault(o => o.DriverID == userId);

        if (order == null)
        {
            error = new Exception("No active order assigned.");
            return false;
        }

        update = new CoordUpdateModel
        {
            UserId = userId,
            Driver = driver,
            Order = order,
            Coords = coords
        };

        return true;
    }
}