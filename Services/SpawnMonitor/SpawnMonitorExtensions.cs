using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using IncursionWebhook.Services.SpawnMonitor.Invocables;

namespace IncursionWebhook.Services.SpawnMonitor
{
    /// <summary>Extension methods for the SpawnMonitor namespace</summary>
    public static class SpawnMonitorExtensions
    {
        /// <summary>Adds the incursion spawn monitor to the <see cref="ServiceCollection"/></summary>
        public static void AddSpawnMonitor(this IServiceCollection services)
        {
            services.AddScheduler();
            services.AddQueue();

            #region Invocables
            services.AddTransient<FetchIncursions>();
            services.AddTransient<SpawnDetected>();
            services.AddTransient<SpawnStateChanged>();
            services.AddTransient<SpawnEnded>();
            #endregion
        }

        /// <summary>Update incursion spawns, every five minutes</summary>
        public static void ScheduleSpawnMonitor(this IServiceProvider serviceProvider)
        {
            serviceProvider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<FetchIncursions>()
                    .EveryMinute()
                    .RunOnceAtStart()
                    .PreventOverlapping("fetchIncursions");
            })
            .LogScheduledTaskProgress(serviceProvider.GetService<ILogger<IScheduler>>());
        }
    }
}
