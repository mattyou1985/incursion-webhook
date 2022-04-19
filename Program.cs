using Coravel;
using IncursionWebhook.Jobs;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            #region Seed Database
            using (var scope = host.Services.CreateScope())
            {
                IServiceProvider? services = scope.ServiceProvider;
                try
                {
                    IRedis? redis = services.GetRequiredService<IRedis>();
                    await DbSeeder.InitializeAsync(redis);
                }
                catch (Exception ex)
                {
                    // Need to work out what I want to do here,
                    // for now we shall re throw the error
                    throw ex;
                }
            }
            #endregion

            #region Scheduler
            host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<FetchIncursions>()
                    .EveryFiveMinutes()
                    .RunOnceAtStart()
                    .PreventOverlapping("fetchIncursions");

                // todo: fetch killmails
            });
            #endregion

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureLogging(logging =>
            {
                logging.AddSimpleConsole(opt =>
                {
                    opt.UseUtcTimestamp = true;
                    opt.IncludeScopes = true;
                });
            });

            // This isn't technically standard in. Net6
            // however I like it as it keeps Startup and Program clean 
            hostBuilder.ConfigureWebHostDefaults(webBuilder => 
                webBuilder.UseStartup<Startup>());

            return hostBuilder;
        }
    }
}