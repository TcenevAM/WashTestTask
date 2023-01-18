using Data.Models;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WashTestTask.Configurations;
using WashTestTask.Database;
using WashTestTask.Services;
using WashTestTask.Services.Interfaces;

namespace WashTestTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<ISalesPointService, SalesPointService>();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            services.AddSwaggerGen();
            services.AddControllers();
            
            var routingSection = Configuration.GetSection("RoutingConfiguration");
            var routingConfig = routingSection.Get<RoutingConfiguration>();

            var rabbitMqSection = Configuration.GetSection("RabbitMqConfiguration");
            var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();

            services.AddDbContext<Context>
                (o => o.UseInMemoryDatabase("MyDatabase"));

            services.AddMassTransit(cfg =>
            {
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(op =>
            {
                op.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
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