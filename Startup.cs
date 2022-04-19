using Coravel;
using IncursionWebhook.Jobs;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook
{
    public class Startup
    {
        private IWebHostEnvironment Environment;

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddScheduler();
            services.AddQueue();

            // Custom services
            services.AddDiscord();
            services.AddEveSwagger();
            services.AddRedis();

            // Add custom jobs
            services.AddTransient<FetchIncursions>();
            services.AddTransient<IncursionSpawned>();
            services.AddTransient<IncursionSpawnDown>();
            services.AddTransient<IncursionStateChange>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}