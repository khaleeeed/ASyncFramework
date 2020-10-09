using ASyncFramework.Application;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure;
using ASyncFramework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Publisher.Filters;

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
            services.AddNestInfrastructure(Configuration);
            services.AddRabbitMQInfrastructure(Configuration);
            services.AddElasticSerilog(Configuration);

            services.AddControllers(options => options.Filters.Add(new ApiExceptionFilter()))
                .AddNewtonsoftJson();
               
            services.AddOpenApiDocument(configure =>
            {                
                configure.Title = "Publisher API";
                configure.AllowReferencesWithProperties = true;
                configure.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;
                configure.DefaultEnumHandling = NJsonSchema.Generation.EnumHandling.String;
            });


            //services.AddHealthChecks();
            services.AddHealthChecks().AddRabbitMQ((s) =>
            {
                var app = s.GetService<IOptionsSnapshot<AppConfiguration>>().Get(AppConfiguration.RabbitMq);
                return new RabbitMQ.Client.ConnectionFactory() { HostName = app.Host, Password = app.Password, UserName = app.UserName };
            });
            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHealthChecks("/health");

            app.UseHttpsRedirection();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
      
    }
}
