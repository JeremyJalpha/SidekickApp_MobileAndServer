using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace SidekickApp.Services
{
    public class CustomToastService
    {
        private readonly IJSRuntime _jsRuntime;

        public CustomToastService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ShowSuccess(string message, int duration = 3000, string? position = null)
        {
            await ShowToast(message, LogLevel.Information, duration, position);
        }

        public async Task ShowError(string message, int duration = 3000, string? position = null)
        {
            await ShowToast(message, LogLevel.Error, duration, position);
        }

        public async Task ShowWarning(string message, int duration = 3000, string? position = null)
        {
            await ShowToast(message, LogLevel.Warning, duration, position);
        }

        public async Task ShowInfo(string message, int duration = 3000, string? position = null)
        {
            await ShowToast(message, LogLevel.Information, duration, position);
        }

        public async Task ShowToast(string message, LogLevel loglvl = LogLevel.Information, int duration = 3000, string? position = null)
        {
            await _jsRuntime.InvokeVoidAsync("showToast", message, loglvl.ToString().ToLower(), duration, position);
        }
    }
}
