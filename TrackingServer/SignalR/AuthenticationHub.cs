using Microsoft.AspNetCore.SignalR;
using TrackingServer.Services;

namespace TrackingServer.SignalR
{
    public class AuthenticationHub : Hub
    {
        private readonly JwtService jwtService;

        public AuthenticationHub(JwtService jwtservice)
        {
            jwtService = jwtservice;
        }

        public async Task<(string?, Exception?)> FromAuthHubGetLongLivedJWT(string shortLivedToken)
        {
            if (string.IsNullOrWhiteSpace(shortLivedToken))
                return (null, new Exception("SLT is null or empty"));

            var phoneNumber = MAS_Shared.Security.JwtHelperUtil.ExtractClaim(shortLivedToken, "phone_number");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return (null, new Exception("Phone number claim is missing from SLT"));

            var jwtResult = await jwtService.GenerateJwtAsync(phoneNumber);
            if (jwtResult.Item2 != null)
                return (null, jwtResult.Item2);

            return (jwtResult.Item1, null);
        }
    }
}
