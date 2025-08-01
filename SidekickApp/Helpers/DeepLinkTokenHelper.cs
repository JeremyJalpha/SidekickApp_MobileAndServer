using MAS_Shared.MASConstants;
using MAS_Shared.Security;
using Microsoft.Extensions.Logging;
using SidekickApp.Services;
using SidekickApp.SignalR;

namespace SidekickApp.Helpers
{
    public static class DeepLinkTokenHelper
    {
        // Validate + connect (the "lens")
        public static async Task<bool> TryUpgradeTokenInnerAsync(
            string token,
            SignalRService signalR,
            ILogger logger)
        {
            if (!JwtHelperUtil.ClientSideJWTPrevalidation(
                    token,
                    MASConstants.ExpectedAudiences,
                    MASConstants.ExpectedIssuer))
            {
                logger.LogWarning("🔭 Token prevalidation failed.");
                return false;
            }

            if (!signalR.IsConnected)
            {
                logger.LogInformation("🔭 SignalR not connected — initiating AuthHub connection.");
                await signalR.StartConnWithJWTAsync(MASConstants.AuthHubUrl, token);
                await Task.Delay(250);
            }

            return true;
        }

        // Upgrade + feedback (the "scope")
        public static async Task<bool> TryUpgradeTokenAsync(
            string shortLivedToken,
            SignalRService signalR,
            ILogger logger,
            AppStateTracker appState)
        {
            var innerPassed = await TryUpgradeTokenInnerAsync(shortLivedToken, signalR, logger);
            if (!innerPassed)
            {
                appState.SetDeferredToast(LogLevel.Error, "Your session link was invalid or expired.");
                return false;
            }

            var (longLivedToken, error) = await signalR.FromAuthHubGetLongLivedJWT(shortLivedToken);

            if (!string.IsNullOrEmpty(longLivedToken) && error == null)
            {
                logger.LogInformation("✅ Token upgraded via AuthHub.");
                await JWTService.StoreTokenAsync(longLivedToken, JwtTokenType.LongLived);
                appState.SetDeferredToast(LogLevel.Information, "Welcome back! You were signed in via deep link.");
                return true;
            }

            logger.LogWarning("⚠️ Token upgrade failed ➜ {Error}", error?.Message);
            appState.SetDeferredToast(LogLevel.Error, "Issue signing in from your deep link.");
            return false;
        }
    }
}