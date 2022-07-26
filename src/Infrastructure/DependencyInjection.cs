using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Service;
using ASyncFramework.Infrastructure.Persistence;
using ASyncFramework.Infrastructure.Persistence.DapperRepo.Repository;
using ASyncFramework.Infrastructure.Persistence.Identity;
using ASyncFramework.Infrastructure.Persistence.Identity.Configuration;
using ASyncFramework.Infrastructure.Persistence.Identity.Repo;
using ASyncFramework.Infrastructure.Persistence.LoggingRepo;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem;
using ASyncFramework.Infrastructure.Persistence.NestRepo;
using ASyncFramework.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nest;
using RabbitMQ.Client;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure
{
    public static class DependencyInjection
    {      
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IGetIPAddress, GetIPAddress>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IReferenceNumberService, ReferenceNumberService>();
            services.AddScoped<IAllHeadersPerRequest, AllHeadersPerRequest>();
        }

        public static void AddDapperInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IDisposeRepository, DisposeRepository>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();
            services.AddSingleton<IPushFailuerRepository, PushFailuerRepository>();
            services.AddSingleton<ICallBackFailureRepository,CallBackFauilerRepository>();
            services.AddSingleton<ITargetFailuerRepository, TargetFailuerRepository>();
            services.AddSingleton<IQueueConfigurationRepository, QueueConfigurationRepository>();
            services.AddSingleton<ISubscriberRepository, SubscriberRepository>();
            services.AddSingleton<IServiceRepository, ServiceRepository>();
            services.AddSingleton<ISystemRepository, SystemRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
        }

        public static void AddRabbitMQInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppConfiguration>(AppConfiguration.RabbitMq,
                                     configuration.GetSection("RabbitMQConfiguration"));
            
            services.AddSingleton<IQueueConfigurationService, QueueConfigurationService>();
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
            services.AddSingleton(typeof(IInfrastructureLogger<>), typeof(ElkLogger<>));
        }

        public static void AddElasticSerilog(this IServiceCollection services,IConfiguration configuration)
        {
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Console.WriteLine(msg);
            });

            Serilog.Core.ILogEventSink failureSink = null;
            Serilog.Configuration.LoggerSinkConfiguration.Wrap(
                new LoggerConfiguration().WriteTo,
                target => failureSink = target,
                x => x.File("./LOG/elkfailure.log", rollingInterval: RollingInterval.Day), Serilog.Events.LogEventLevel.Debug, new Serilog.Core.LoggingLevelSwitch());

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("ReferenceNumber", string.Empty)
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .Filter.ByExcluding(Matching.FromSource("System"))
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Host"]))
                {

                    ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticConfiguration:UserName"],
                    configuration["ElasticConfiguration:Password"]).ServerCertificateValidationCallback(
                        new Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool>(delegate { return true; })),

                    IndexFormat = "log-integ-system-asyncfreamowrk-{0:yyyy-MM}",
                    RegisterTemplateFailure = RegisterTemplateRecovery.FailSink,
                    EmitEventFailure = EmitEventFailureHandling.WriteToFailureSink,
                    FailureSink = failureSink

                })
                .CreateLogger();
        }

        public static void AddIdentityLogin(this IServiceCollection services, IConfiguration configuration)
        {            
            // Adding Identity
            services.AddDefaultIdentity<AsyncUser>()
                .AddUserStore<AsyncUserStoreService>()
                .AddUserManager<AsyncUserManager>();
                

            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Query.ContainsKey("access_token"))
                        {
                            context.Token = context.Request.Query["access_token"];
                        }

                        return Task.CompletedTask;
                    }
                };
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = ConfigurationJWT.Aduince,
                    ValidIssuer = ConfigurationJWT.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationJWT.Key))
                };
            });


            services.Configure<AppConfiguration>(AppConfiguration.ActiveDirectoryApi, configuration.GetSection(AppConfiguration.ActiveDirectoryApi));
            services.Configure<AppConfiguration>(AppConfiguration.EmailConfirmationApi, configuration.GetSection(AppConfiguration.EmailConfirmationApi));
            services.Configure<AppConfiguration>(AppConfiguration.MojSystemApi, configuration.GetSection(AppConfiguration.MojSystemApi));

            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ISendEmailService, SendEmailService>();
            services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();
            services.AddScoped<IMojSystemService, MojSystemService>();

        }

        public static IHostBuilder UseElasticSerilog(this IHostBuilder app)
        {
            return app.UseSerilog();
        }
    }
}