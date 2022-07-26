using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ASyncFramework.Application;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure;
using HealthChecks.UI.Client;
using Manager.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AsyncPolicy",
                    builder =>
                    {
                        builder.WithOrigins("http://10.162.1.164:800", "http://10.162.1.165:800", "localhost", "http://10.161.2.164:800", "http://10.161.2.165:800", "http://10.160.7.90:800")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });


            services.AddApplication();
            services.AddMediatR();
            services.AddRabbitMQInfrastructure(Configuration);
            services.AddNestInfrastructure(Configuration);
            services.AddDapperInfrastructure();
            services.AddIdentityLogin(Configuration);
            services.AddInfrastructure();
            services.AddElasticSerilog(Configuration);


            services.AddControllersWithViews(options => options.Filters.Add(new ApiExceptionFilter()))
               .AddNewtonsoftJson()
               .AddJsonOptions(options=>options.JsonSerializerOptions.IgnoreNullValues=true)
               .AddRazorRuntimeCompilation();
            

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


            services.AddHealthChecks();
            
            services.AddHealthChecks().AddRabbitMQ((s) =>
            {
                var app = s.GetService<IOptionsSnapshot<AppConfiguration>>().Get(AppConfiguration.RabbitMq);
                return new RabbitMQ.Client.ConnectionFactory() { HostName = app.Host, Password = app.Password, UserName = app.UserName };
            });

            services.AddHealthChecks().AddSqlServer(Configuration.GetConnectionString("DefaultConnection"));

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseCors("AsyncPolicy");


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
