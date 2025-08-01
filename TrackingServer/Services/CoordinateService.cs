using MAS_Shared.Models;
using MAS_Shared.Data;
using TrackingServer.Interfaces;
using TrackingServer.Models;

namespace TrackingServer.Services
{
    public class CoordinateService : ICoordinateService
    {
        private readonly AppDbContext dbContext;

        public CoordinateService(AppDbContext dbcontext)
        {
            dbContext = dbcontext;
        }

        public async Task<Exception?> HandleCoordinates(CoordUpdateModel coordUpdate)
        {
            // Validate the coordinates
            var coordValidationOp = ValidateCoordinates(coordUpdate.Coords);
            if (coordValidationOp != null)
            {
                return await Task.FromResult(coordValidationOp);
            }

            // Save the coordinates to the database
            var coordSaveOp = SaveCoordinatesToDb(coordUpdate);

            // Apply business logic
            var updateOrderOp = UpdateOrderStatus(coordUpdate);

            return await Task.FromResult(coordSaveOp);
        }

        private Exception? SaveCoordinatesToDb(CoordUpdateModel coordUpdate)
        {
            try
            {
                if (coordUpdate.Driver == null)
                    return new Exception("Driver information is missing in the coordinate update.");
                coordUpdate.Driver.GPSLat = coordUpdate.Coords.Latitude;
                coordUpdate.Driver.GPSLong = coordUpdate.Coords.Longitude;
                coordUpdate.Driver.LocationLastUpdated = DateTime.Now;
                dbContext.SaveChanges();
                return null;
            }
            catch (Exception)
            {
                return new Exception("Error saving coordinates to the database.");
            }
        }

        private Exception? UpdateOrderStatus(CoordUpdateModel coordUpdate)
        {
            // Implement business logic
            return null;
        }

        private Exception? ValidateCoordinates(GPSLocationDTO coords)
        {
            if (coords == null)
                return new Exception("Coordinates were null or empty");
            return null;
        }
    }
}
