using ASyncFramework.Application;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Common;
using ASyncFramework.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Publisher.Filters;
using Publisher.Service;

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
            services.AddApplication();
            services.AddInfrastructure(Configuration);


            services.AddControllers(options=>options.Filters.Add(new ApiExceptionFilter()));

            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Publisher API";
                configure.AllowReferencesWithProperties = true;
                configure.DefaultEnumHandling = NJsonSchema.Generation.EnumHandling.String;

            });

            //services.AddHealthChecks();
            services.AddHealthChecks().AddRabbitMQ((s) => { var app = s.GetService<IOptions<AppConfiguration>>();return new RabbitMQ.Client.ConnectionFactory() { HostName = app.Value.RabbitHost, Password = app.Value.RabbitPassword, UserName = app.Value.RabbitUserName };});

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IReferenceNumberService, ReferenceNumberService>();
            services.AddScoped<IAllHeadersPerRequest, AllHeadersPerRequest>();

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
