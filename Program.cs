using IncursionWebhook.Services.Redis;
using IncursionWebhook.Services.SpawnMonitor;

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
                    DbSeeder.Initialize(redis);
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168
                {
                    // Need to work out what I want to do here,
                    // for now we shall re throw the error
                    throw;
                }
            }
            #endregion

            #region Scheduler
            host.Services.ScheduleSpawnMonitor();
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