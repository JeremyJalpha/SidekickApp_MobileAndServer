using MAS_Shared.MASConstants;
using MAS_Shared.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using SidekickApp.Services;
using SidekickApp.SignalR;

namespace SidekickApp.Helpers
{
    public static class SignalRAuthHelper
    {
        public static async Task<bool> EnsureConnectedWithGPSAsync(
            AuthenticationStateProvider authProvider,
            SignalRService signalRService,
            CustomToastService toastService,
            ILogger logger)
        {
            var authState = await authProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!(user.Identity?.IsAuthenticated ?? false))
            {
                await toastService.ShowToast("You're not signed in yet.", LogLevel.Warning);
                logger.LogWarning("GPS ➜ User not authenticated.");
                return false;
            }

            var token = await JWTService.RetrieveTokenAsync();
            if (string.IsNullOrEmpty(token) ||
                !JwtHelperUtil.ClientSideJWTPrevalidation(token, MASConstants.ExpectedAudiences, MASConstants.ExpectedIssuer))
            {
                await toastService.ShowToast("Authentication required to share location.", LogLevel.Warning);
                logger.LogWarning("GPS ➜ Token invalid or missing.");
                return false;
            }

            await signalRService.StartConnWithJWTAsync(MASConstants.GPSHubUrl, token);

            if (!signalRService.IsConnected)
            {
                await toastService.ShowToast("Unable to connect to GPSHub.", LogLevel.Warning);
                logger.LogError("GPS ➜ SignalR connection to GPSHub failed.");
                return false;
            }

            return true;
        }
    }
}