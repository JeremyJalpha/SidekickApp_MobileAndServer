using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using SidekickApp.Services;
using SidekickApp.SignalR;

namespace SidekickApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<AppStateTracker>();
            builder.Services.AddScoped<CustomToastService>();
            builder.Services.AddScoped<AuthenticationStateProvider, SidekickAuthProvider>();
            builder.Services.AddSingleton<SignalRService>();
            var sink = new UILogSink();
            builder.Services.AddSingleton(sink);
            builder.Logging.AddProvider(new UILoggerProvider(sink));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
