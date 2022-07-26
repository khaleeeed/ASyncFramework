using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Plugins.RecentHistory;
using Quartz.Spi;
using QuartzManager.JobManager;
using Quartzmin;

namespace QuartzManager
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
            services.AddInfrastructure();
            services.AddRabbitMQInfrastructure(Configuration);
            services.AddElasticSerilog(Configuration);
            services.AddDapperInfrastructure();
            services.AddNestInfrastructure(Configuration);

          

            services.AddHostedService<QuartzHostedService>();            
            
            services.AddQuartzmin();
            
            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add job
            services.AddSingleton<RecoveryFauilerJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(RecoveryFauilerJob),
                cronExpression: "0 */30 * ? * *")); //Every 30 minutes

            
            services.AddControllers();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. QuartzminPlugin
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseQuartzmin(new QuartzminOptions()
            {                
                Scheduler = StdSchedulerFactory.GetDefaultScheduler().Result     
                
            });
            
           
            app.UseHttpsRedirection();

            app.UseRouting();
           
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}