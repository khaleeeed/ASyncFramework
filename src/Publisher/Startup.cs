using ASyncFramework.Application;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Publisher.Filters;
using System;
using System.Net.Http;
using System.Text;

namespace Publisher
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

            services.AddPublisherMediatR();
            services.AddApplication();
            services.AddMediatR();
            services.AddInfrastructure();
            services.AddRabbitMQInfrastructure(Configuration);
            services.AddNestInfrastructure(Configuration);
            services.AddElasticSerilog(Configuration);
            services.AddDapperInfrastructure();

            services.AddControllers(options => options.Filters.Add(new ApiExceptionFilter()))
                .AddNewtonsoftJson();
               
            services.AddOpenApiDocument(configure =>
            {                
                configure.Title = "Publisher API";
                configure.AllowReferencesWithProperties = true;
                configure.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;
                configure.DefaultEnumHandling = NJsonSchema.Generation.EnumHandling.String;
            });


            services.AddHealthChecks();
            services.AddHealthChecks().AddRabbitMQ((s) =>
            {
                var app = s.GetService<IOptionsSnapshot<AppConfiguration>>().Get(AppConfiguration.RabbitMq);
                return new RabbitMQ.Client.ConnectionFactory() { HostName = app.Host, Password = app.Password, UserName = app.UserName };
            });

            services.AddHealthChecks().AddSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            
            services.AddTransient(typeof(IIdentityRepository), sp => null);
            services.AddTransient(typeof(ITokenRepository), sp => null);
            services.AddTransient(typeof(ISendEmailService), sp => null);
            services.AddTransient(typeof(IActiveDirectoryService), sp => null);
            services.AddTransient(typeof(IMojSystemService), sp => null);       
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           
            app.UseHttpsRedirection();
            app.UseOpenApi();
            app.UseSwaggerUi3();
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