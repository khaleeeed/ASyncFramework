using ASyncFramework.Application;
using ASyncFramework.Application.SubscribeRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem.QueueSubscriber;
using ASyncFramework.Infrastructure.Service;
using Elastic.Apm.NetCoreAll;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;

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
            services.AddTransient<ISubscriberLogic, SubscriberLogic>();
            services.AddDapperInfrastructure();
            services.AddElasticSerilog(Configuration);
            services.AddNestInfrastructure(Configuration);

            // start background process for subscrib 
            services.AddSingleton<RunTimeQueue>();
            services.AddHostedService(sp => sp.GetRequiredService<RunTimeQueue>());

            services.AddTransient(typeof(ICurrentUserService), sp => null);
            services.AddTransient(typeof(IReferenceNumberService), sp => null);
            services.AddTransient(typeof(IAllHeadersPerRequest), sp => null);
            services.AddSingleton<IASyncAgent, ASyncAgent>();
            services.AddControllers();

            services.AddHealthChecks();
            services.AddHealthChecks().AddRabbitMQ((s) =>
            {
                var app = s.GetService<IOptionsSnapshot<AppConfiguration>>().Get(AppConfiguration.RabbitMq);
                return new RabbitMQ.Client.ConnectionFactory() { HostName = app.Host, Password = app.Password, UserName = app.UserName };
            });

            services.AddHealthChecks().AddSqlServer(Configuration.GetConnectionString("DefaultConnection"));


            services.AddHealthChecks().AddCheck("Elasticsearch", () =>
            {
                string userName = Configuration["ElasticConfiguration:UserName"];
                string password = Configuration["ElasticConfiguration:Password"];
                string host = Configuration["ElasticConfiguration:Host"];
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));

                using HttpClientHandler clientHandler = new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                };
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using var clinet = new HttpClient(clientHandler);
                clinet.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
                var result = clinet.GetAsync(host).Result;
                if (result.IsSuccessStatusCode)
                    return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy, description: $"elastic {host}");
                else
                    return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, description: result.Content.ReadAsStringAsync().Result);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {                        
            app.UseAllElasticApm(Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse                
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}