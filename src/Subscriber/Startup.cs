using ASyncFramework.Application;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.SubscribeRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure;
using ASyncFramework.Infrastructure.Persistence;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem.QueueSubscriber;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Subscriber
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
            services.AddApplication();
            services.AddInfrastructure();
            services.AddRabbitMQInfrastructure(Configuration);
            services.AddElasticSerilog(Configuration);
            services.AddNestInfrastructure(Configuration);
            services.AddTransient<ISubscriberLogic, SubscriberLogic>();
            services.AddHostedService<DirectQueue>();
            services.AddHostedService<CallBackFailuerQueue>();
            services.AddHostedService<RunTimeQueue>();
            services.AddTransient(typeof(ICurrentUserService),sp=> null);
            services.AddTransient(typeof(IReferenceNumberService), sp => null);
            services.AddTransient(typeof(IAllHeadersPerRequest), sp => null);
            services.AddControllers();

            services.AddHealthChecks().AddRabbitMQ((s) => 
            {
                var app = s.GetService<IOptionsSnapshot<AppConfiguration>>().Get(AppConfiguration.RabbitMq);
                return new RabbitMQ.Client.ConnectionFactory() { HostName = app.Host, Password = app.Password, UserName = app.UserName };
            });
                      
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHealthChecks("/health");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}