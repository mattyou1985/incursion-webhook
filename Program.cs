namespace IncursionWebhook
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            #region Seed Database
            // TODO: Use Redis or SQL?  | Seed DB with universe information (Systems, Constellations, Regions)
            #endregion

            #region Scheduler
            // TODO: Add Coravel
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