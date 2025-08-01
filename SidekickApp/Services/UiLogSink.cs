using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace SidekickApp.Services
{
    public class UILogSink
    {
        private readonly ObservableCollection<UILogEntry> _entries = new();
        private readonly object _lock = new();

        public ReadOnlyObservableCollection<UILogEntry> Entries { get; }

        public UILogSink()
        {
            Entries = new(_entries);
        }

        public void Add(LogLevel level, string category, string message, Exception? ex = null)
        {
            lock (_lock)
            {
                var entry = new UILogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = level,
                    Category = category,
                    Message = message,
                    Exception = ex?.ToString()
                };

                _entries.Add(entry);
                if (_entries.Count > 100)
                    _entries.RemoveAt(0);
            }
        }
    }

    public class UILogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
    }
}