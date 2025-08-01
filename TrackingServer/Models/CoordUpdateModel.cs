using MAS_Shared.Models;
using MAS_Shared.Data;

namespace TrackingServer.Models
{
    public class CoordUpdateModel
    {
        public string UserId { get; set; } = string.Empty;
        public Driver? Driver { get; set; }
        public Order? Order { get; set; }
        public GPSLocationDTO Coords { get; set; } = new GPSLocationDTO();
    }
}