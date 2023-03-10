using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SaleCreationService.Configurations;
using SaleCreationService.Consumers;
using SaleCreationService.Services;

namespace SaleCreationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISaleService, SaleService>();
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "SaleCreationService", Version = "v1" }); });

            var rabbitMqSection = Configuration.GetSection("RabbitMqConfiguration");
            var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();
            
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SaleCreationHandler>();
                
                cfg.UsingRabbitMq((context, config) =>
                {
                    config.ConfigureEndpoints(context);

                    config.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(op =>
            {
                op.SwaggerEndpoint("/swagger/v1/swagger.json", "SaleCreationService v1");
                op.RoutePrefix = string.Empty;
            });
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{id?}");
                endpoints.MapControllers();
            });
        }
    }
}