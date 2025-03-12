using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.MassTransitQueue;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Depository.MassTransit
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
            var configSections = Configuration.GetSection("Rabbitmq");
            var host = configSections["Host"];
            var userName = configSections["UserName"];
            var password = configSections["Password"];
            var virtualHost = configSections["VirtualHost"];
            var port = Convert.ToUInt16(configSections["Port"]);

            services.AddSwaggerGen();
            services.AddMassTransit(a =>
            {
                a.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(host, "/", h =>
                    {
                        h.Username(userName);
                        h.Password(password);

                        h.Heartbeat(TimeSpan.FromSeconds(3));

                      //  h.RequestedConnectionTimeout(TimeSpan.FromSeconds(3));
                    });

                });

                a.AddConsumer<EventConsumer>();
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Depository Pass Actions Result To Frontend  API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
