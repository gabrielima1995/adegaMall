using Hangfire.States;
using System;

namespace Infra.Crosscutting.Hangfire
{
    public class HangfireSettings
    {
        public string ConnectionString { get; set; }

        public string[] Queues { get; set; }

        public string PanelUrl { get; set; }

        public int WorkersCount { get; set; }

        public TimeSpan SchedulePollingInterval { get; set; }

        public HangfireSettings()
        {
            ConnectionString = ConfigHelper.GetSetting("HANGFIRE:CONNSTRING:NAME", "Hangfire");
            Queues = ConfigHelper.GetSetting("HANGFIRE:QUEUES", EnqueuedState.DefaultQueue).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            PanelUrl = ConfigHelper.GetSetting("HANGFIRE:PANEL:URL", "/notouchme/hangfire");
            WorkersCount = ConfigHelper.GetSetting("HANGFIRE:WORKERS", Environment.ProcessorCount);
            SchedulePollingInterval = TimeSpan.FromMinutes(ConfigHelper.GetSetting("HANGFIRE:SCHEDULE:POLLING:INTERVAL", 15D));
        }
    }
}
