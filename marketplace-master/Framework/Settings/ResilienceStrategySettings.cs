using System;

namespace Framework.Settings
{
    public class ResilienceStrategySettings
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.Parse("00:00:30");
        public TimeSpan[] RetrySleeps { get; set; }
        public int EventsBeforeBreaking { get; set; } = 5;
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.Parse("00:00:30");
    }
}