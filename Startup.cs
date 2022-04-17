using Coravel;
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
            services.AddDiscordWebhook();
            services.AddEveSwagger();
            services.AddRedis();

            // Add custom jobs
            //services.AddSingleton<>();
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