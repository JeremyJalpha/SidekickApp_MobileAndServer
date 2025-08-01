using TrackingServer.Models;

namespace TrackingServer.Interfaces
{
    public interface ICoordinateService
    {
        Task<Exception?> HandleCoordinates(CoordUpdateModel coordUpdate);
    }
}
