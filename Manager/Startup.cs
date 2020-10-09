using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Application;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Infrastructure;
using Manager.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Manager
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
            services.AddMediatR();
            services.AddRabbitMQInfrastructure(Configuration);
            services.AddNestInfrastructure(Configuration);
            services.AddInfrastructure();
            services.AddControllers(options => options.Filters.Add(new ApiExceptionFilter()))
               .AddNewtonsoftJson()
               .AddJsonOptions(options=>options.JsonSerializerOptions.IgnoreNullValues=true);
            

            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Manager API";
                configure.AllowReferencesWithProperties = true;
                configure.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;
                configure.DefaultEnumHandling = NJsonSchema.Generation.EnumHandling.String;
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
