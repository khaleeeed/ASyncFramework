using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using ASyncFramework.Infrastructure.Persistence.LoggingRepo;
using ASyncFramework.Infrastructure.Persistence.QueueSystem;
using ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASyncFramework.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppConfiguration>(options => configuration.GetSection("RabbitMQConfiguration").Bind(options));
            services.Configure<Dictionary<string, QueueConfiguration>>(options => configuration.GetSection("QueueConfiguration").Bind(options));
            services.AddTransient<IRabbitMQPersistent, RabbitMQPersistent>();
            services.AddSingleton<IRabbitProducers, RabbitProducers>();
            services.AddTransient(typeof(IElkLogger<>), typeof(ElkLogger<>));


            Log.Logger = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("ReferenceNumber", string.Empty)
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .Filter.ByExcluding(Matching.FromSource("System"))
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
                {
                    AutoRegisterTemplate = true,
                    ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticConfiguration:UserName"], configuration["ElasticConfiguration:Password"]),
                    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{DateTime.Now:yyyy-MM}"
                })
                .CreateLogger();
            
        }

        public static IHostBuilder UseElasticSerilog(this IHostBuilder app)
        {
            return app.UseSerilog();
        }
    }
}
