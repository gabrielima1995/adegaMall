using Domain.Interfaces;
using Hangfire;
using Infra.SmartSearchAdapter;
using Owin;
using System;

namespace Presentation.Configuration
{
    public static class SmartSearchConfigurationExtensions
    {
        public static IAppBuilder UseSmartSearch(this IAppBuilder app)
        {
            var backgroundWorker = UnityConfig.Resolve<IBackgroundWorker>();

            backgroundWorker.Schedule<IndexUpdater>(x => x.CreateOrUpdateIndexes(), TimeSpan.FromMinutes(5));
            backgroundWorker.CreateRecurring<IndexUpdater>(x => x.CreateOrUpdateIndexes(), "UpdateIndicesJob", Cron.Daily(3));

            return app;
        }
    }
}