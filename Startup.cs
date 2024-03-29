﻿using Coravel;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger;
using IncursionWebhook.Services.Redis;
using IncursionWebhook.Services.SpawnMonitor;
using Microsoft.AspNetCore.HttpOverrides;
using System.Reflection;

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
            services.AddSwaggerGen(options =>
            {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            });
            services.AddScheduler();
            services.AddQueue();

            // Custom services
            services.AddDiscord();
            services.AddEveSwagger();
            services.AddRedis();
            services.AddSpawnMonitor();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Enable reverse proxy to work
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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