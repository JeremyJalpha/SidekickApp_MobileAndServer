using Android.App;
using Android.Content.PM;
using Android.OS;
using Intent = Android.Content.Intent;

using SidekickApp.Services;
using SidekickApp.SignalR;
using Microsoft.Extensions.Logging;
using SidekickApp.Helpers;

namespace SidekickApp
{
    [Activity(
        Exported = true,
        LaunchMode = LaunchMode.SingleTop,
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize |
            ConfigChanges.Orientation |
            ConfigChanges.UiMode |
            ConfigChanges.ScreenLayout |
            ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(
        [Intent.ActionView],
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "xyz.sidekickapp",
        DataHost = "login")]
    public class MainActivity : MauiAppCompatActivity
    {
        private ILogger<MainActivity>? _logger;
        private AppStateTracker? _appState;
        private SignalRService? _signalR;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _logger = IPlatformApplication.Current?.Services?.GetService<ILogger<MainActivity>>();
            _appState = IPlatformApplication.Current?.Services?.GetService<AppStateTracker>();
            _signalR = IPlatformApplication.Current?.Services?.GetService<SignalRService>();

            if (Intent != null)
            {
                HandleIntent(Intent);
                _logger?.LogInformation("📥 Deep link intent received via OnCreate.");
            }
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            if (intent == null)
            {
                _logger?.LogWarning("OnNewIntent ➜ Null intent received.");
                return;
            }

            HandleIntent(intent);
        }

        private void CheckAppStateAndToast(LogLevel toastLevel, string toastMsg)
        {
            if (_appState != null)
            {
                _appState.SetDeferredToast(toastLevel, toastMsg);
            }
            else
            {
                _logger?.LogWarning("CheckAppStateAndToast ➜ AppStateTracker is null, cannot set deferred toast.");
            }
        }

        private async void HandleIntent(Intent intent)
        {
            if (intent?.Data == null)
            {
                _logger?.LogWarning("HandleIntent ➜ Intent has no Data.");
                return;
            }

            try
            {
                var uri = new Uri(intent.Data.ToString()!);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var token = queryParams.Get("token");

                if (string.IsNullOrEmpty(token))
                {
                    _logger?.LogWarning("HandleIntent ➜ No token found in query params.");
                    return;
                }

                _logger?.LogInformation("🔗 Token extracted from deep link ➜ {Token}", token);

                if (_signalR == null)
                {
                    _logger?.LogWarning("HandleIntent ➜ SignalRService is null, cannot connect.");
                    CheckAppStateAndToast(LogLevel.Error, "SignalR service not available.");
                    return;
                }

                await DeepLinkTokenHelper.TryUpgradeTokenAsync(
                    token,
                    _signalR,
                    _logger!,
                    _appState!
                );
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ [MainActivity] Deep link handling error");
                CheckAppStateAndToast(LogLevel.Error, "There was an error handling the deep link.");
            }

            _logger?.LogInformation("🎯 Deep link handling complete.");
        }
    }
}