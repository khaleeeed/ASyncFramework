using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Respawn;
using System.IO;

namespace ApplicationTests.IntegrationTests
{
    [SetUpFixture]
    public class Testing
    {
        private static IConfigurationRoot _configuration;
        public void RunBeforeAnyTest()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables();

            _configuration = builder.Build();

            var startup = new Publisher.Startup(_configuration);
            //var startup = new Subscriber.Startup(_configuration);

            var services = new ServiceCollection();

            services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
            w.EnvironmentName == "Development" && w.ApplicationName == "ASyncFramework.Publisher"));

            startup.ConfigureServices(services);
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {

        }
    }
}
