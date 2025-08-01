using MAS_Shared.Models;

namespace SidekickApp.SignalR
{
    public static class GPSLocationService
    {
        public static async Task<GPSLocationDTO> GetCurrentLocationAsync()
        {
            Location? location = null;
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.High,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                });
            }
            // TODO: implement a call back to handle the different errors - for now throw an exception
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
                throw new FeatureNotSupportedException("Unable to get location.");
            }
            catch (FeatureNotEnabledException)
            {
                // Handle not enabled on device exception
                throw new FeatureNotEnabledException("Unable to get location.");
            }
            catch (PermissionException)
            {
                // Handle permission exception
                throw new PermissionException("Unable to get location.");
            }
            catch (Exception)
            {
                // Unable to get location
                throw new Exception("Unable to get location.");
            }

            if (location == null)
                throw new Exception("Unable to get location.");

            return new GPSLocationDTO
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
    }
}
