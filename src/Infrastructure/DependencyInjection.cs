using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Service;
using ASyncFramework.Infrastructure.Persistence;
using ASyncFramework.Infrastructure.Persistence.LoggingRepo;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem;
using ASyncFramework.Infrastructure.Persistence.NestRepo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Nest;
using RabbitMQ.Client;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace ASyncFramework.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IGetIPAddress, GetIPAddress>();
            services.AddSingleton(typeof(IElkLogger<>), typeof(ElkLogger<>));
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IReferenceNumberService, ReferenceNumberService>();
            services.AddScoped<IAllHeadersPerRequest, AllHeadersPerRequest>();
        }
        public static void AddRabbitMQInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppConfiguration>(AppConfiguration.RabbitMq,
                                     configuration.GetSection("RabbitMQConfiguration"));
            
            services.Configure<Dictionary<string, QueueConfiguration>>(options => configuration.GetSection("QueueConfiguration").Bind(options));
            services.AddTransient<IRabbitMQPersistent, RabbitMQPersistent>();

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitMQPersistent>();
            services.AddSingleton<IRabbitProducers, RabbitProducers>();

        }
        public static void AddNestInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<AppConfiguration>(AppConfiguration.Elastic,configuration.GetSection("ElasticConfiguration"));

            services.AddSingleton(typeof(IElasticClient), s =>
            {
                var app = s.GetService<IOptionsMonitor<AppConfiguration>>().Get(AppConfiguration.Elastic);
                
                var settings = new ConnectionSettings(new Uri(app.Host))
                .BasicAuthentication(app.UserName, app.Password)
                .ServerCertificateValidationCallback(
                        new Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool>(delegate { return true; }));

                return new ElasticClient(settings);
            });

            services.AddSingleton<IASyncFrameworkInfrastructureRepository, ASyncFrameworkInfrastructureRepository>();
            services.AddSingleton<ICallBackFailureRepository, CallBackFailureRepository>();
            services.AddSingleton<ITargetFailuerRepository, TargetFailuerRepository>();
            services.AddSingleton<ILUTNotificationRepository, LUTNotificationRepository>();
        }
        public static void AddElasticSerilog(this IServiceCollection services,IConfiguration configuration)
        {
            //Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            //Serilog.Core.ILogEventSink failureSink = null;
            //Serilog.Configuration.LoggerSinkConfiguration.Wrap(
            //    new LoggerConfiguration().WriteTo,
            //    target => failureSink = target,
            //    x => x.File("./failure.log", rollingInterval: RollingInterval.Day), Serilog.Events.LogEventLevel.Debug, new Serilog.Core.LoggingLevelSwitch());

            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.WithMachineName()
            //    .Enrich.WithEnvironmentUserName()
            //    .Enrich.WithProperty("ReferenceNumber", string.Empty)
            //    .Filter.ByExcluding(Matching.FromSource("Microsoft"))
            //    .Filter.ByExcluding(Matching.FromSource("System"))
            //    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Host"]))
            //    {

            //        ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticConfiguration:UserName"],
            //        configuration["ElasticConfiguration:Password"]).ServerCertificateValidationCallback(
            //            new Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool>(delegate { return true; })),

            //        IndexFormat = "test-asyncframework-infrastructure-{0:yyyy-MM}",
            //        RegisterTemplateFailure = RegisterTemplateRecovery.FailSink,
            //        EmitEventFailure = EmitEventFailureHandling.WriteToFailureSink,
            //        FailureSink = failureSink

            //    })
            //    .CreateLogger();

            Serilog.Core.ILogEventSink failureSink = null;
            Serilog.Configuration.LoggerSinkConfiguration.Wrap(
                new LoggerConfiguration().WriteTo,
                target => failureSink = target,
                x => x.File("./failure.log", rollingInterval: RollingInterval.Day), Serilog.Events.LogEventLevel.Debug, new Serilog.Core.LoggingLevelSwitch());

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("ReferenceNumber", string.Empty)
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .Filter.ByExcluding(Matching.FromSource("System"))
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://10.162.1.54:9200"))
                {

                    ModifyConnectionSettings = x => x.BasicAuthentication("admin", "Moj@12345").ServerCertificateValidationCallback(
                        new Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool>(delegate { return true; })),

                    IndexFormat = "test-test-test-{0:yyyy-MM}",
                    RegisterTemplateFailure = RegisterTemplateRecovery.FailSink,
                    EmitEventFailure = EmitEventFailureHandling.WriteToFailureSink,
                    FailureSink = failureSink

                })
                .CreateLogger();
        }
        public static IHostBuilder UseElasticSerilog(this IHostBuilder app)
        {
            return app.UseSerilog();
        }
       
    }
}
