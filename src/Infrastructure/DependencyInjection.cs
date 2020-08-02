using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using ASyncFramework.Infrastructure.Persistence.QueueSystem;
using ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

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
        }
    }
}
