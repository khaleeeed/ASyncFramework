using ASyncFramework.Application;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.SubscribeRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure;
using ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Subscriber.HttpRequestCode;
using Subscriber.Service;

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
            services.AddInfrastructure(Configuration);
            services.AddTransient<ISubscriberLogic, SubscriberLogic>();
            services.AddHostedService<DirectQueue>();
            services.AddHostedService<CallBackFailuerQueue>();
            services.AddHostedService<RunTimeQueue>();

            services.AddControllers();

            services.AddHealthChecks().AddRabbitMQ((s) => { var app = s.GetService<IOptions<AppConfiguration>>(); return new RabbitMQ.Client.ConnectionFactory() { HostName = app.Value.RabbitHost, Password = app.Value.RabbitPassword, UserName = app.Value.RabbitUserName }; });
           
            services.AddHttpContextAccessor();

            services.AddTransient<IConvertRequestToHttpRequestMessage, ConvertRequestToHttpRequestMessage>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IReferenceNumberService, ReferenceNumberService>();
            services.AddTransient<IAllHeadersPerRequest, AllHeadersPerRequest>();
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