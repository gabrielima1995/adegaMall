using System;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Infra.Crosscutting.Hangfire;
using Owin;

namespace Hangfire
{
    /// <summary>
    /// Class with Hangfire configuration methods.
    /// </summary>
    public static class Configuration
    {
        private static void SetStorage(HangfireSettings settings)
        {
            var options = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true,
                JobExpirationCheckInterval = TimeSpan.FromMinutes(10)
            };

            GlobalConfiguration.Configuration.UseSqlServerStorage(settings.ConnectionString, options);
        }

        /// <summary>
        /// Adds Dashboard UI middleware to the OWIN request processing pipeline for the Hangfire.JobStorage.Current storage.
        /// </summary>
        /// <param name="app">OWIN application builder.</param>
        public static void UseHangfirePanel(this IAppBuilder app, HangfireSettings settings = null)
        {
            settings = settings ?? new HangfireSettings();

            SetStorage(settings);

            var options = new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilter() }
            };

            AppBuilderExtensions.UseHangfireDashboard(app, settings.PanelUrl, options);
        }

        /// <summary>
        /// Creates a new instance of the Hangfire.BackgroundJobServer class and Hangfire.JobStorage.Current storage and registers its disposal on application shutdown.
        /// </summary>
        /// <param name="app">OWIN application builder.</param>
        /// <param name="activator">The activator responsible for resolving the IoC dependencies.</param>
        public static void UseHangfireServer(this IAppBuilder app, JobActivator activator, HangfireSettings settings = null)
        {
            settings = settings ?? new HangfireSettings();

            SetStorage(settings);

            GlobalConfiguration.Configuration.UseActivator(activator);

            GlobalJobFilters.Filters.Add(new ProlongExpirationTimeAttribute());

            var options = new BackgroundJobServerOptions
            {
                WorkerCount = settings.WorkersCount,
                Queues = settings.Queues,
                SchedulePollingInterval = settings.SchedulePollingInterval
            };

            AppBuilderExtensions.UseHangfireServer(app, options);
        }
    }
}