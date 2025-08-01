using MAS_Shared.MASConstants;
using Microsoft.Extensions.Logging;
using SidekickApp.Services;
using SidekickApp.SignalR;

namespace SidekickApp
{
    public partial class App : Application
    {
        private readonly AppStateTracker _appState;
        private readonly SignalRService _signalR;

        public App(AppStateTracker stateTracker, SignalRService signalRService)
        {
            InitializeComponent();

            _appState = stateTracker;
            _signalR = signalRService;

            _appState.ConnectSignals(_signalR);
            _ = _appState.InitializeAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var startPage = new MainPage();

            var logger = IPlatformApplication.Current?.Services?.GetService<ILogger<App>>();
            if (logger == null)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Logger<App> not resolved — logging disabled for startup.");
                _appState.SetDeferredToast(LogLevel.Warning, "Some app components did not load properly during startup, please try reloading the app.");
                _appState.RaiseUiHydrated(false);
                return new Window(startPage) { Title = "SidekickApp" };
            }

            var authProvider = IPlatformApplication.Current?.Services?.GetService<SidekickAuthProvider>();
            (string? token, bool isLongLived) = authProvider != null
                ? authProvider.GetValidatedTokenWithSourceAsync().GetAwaiter().GetResult()
                : (null, false);

            logger.LogInformation("🚀 Startup with {Source} ➜ {TokenDescription}",
                isLongLived ? "long-lived" : "short-lived",
                string.IsNullOrEmpty(token) ? "no token" : $"token: {token}");

            if (!string.IsNullOrEmpty(token))
            {
                // Only store short-lived tokens here — long-lived should already exist
                if (!isLongLived)
                    JWTService.StoreTokenAsync(token, JwtTokenType.ShortLived).GetAwaiter().GetResult();

                _appState.SetDeferredToast(LogLevel.Information, "Welcome back! You were signed in via deep link.");
            }
            else
            {
                logger.LogWarning("❌ Deep link token was invalid or missing.");
                _appState.SetDeferredToast(LogLevel.Error, "Your deep link session was invalid or expired.");
            }

            _appState.RaiseUiHydrated(true);

            return new Window(startPage) { Title = "SidekickApp" };
        }
    }
}