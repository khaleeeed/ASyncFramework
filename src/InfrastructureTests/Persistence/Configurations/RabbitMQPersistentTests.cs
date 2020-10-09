using ASyncFramework.Infrastructure.Persistence.Configurations;
using ASyncFramework.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;

namespace ASyncFramework.Infrastructure.Persistence.Configurations.Tests
{
    [TestFixture()]
    public class RabbitMQPersistentTests
    {
        Mock<IOptions<AppConfiguration>> options = new Mock<IOptions<AppConfiguration>>();
        Mock<RabbitMQPersistent> rabbitMQPersistent = new Mock<RabbitMQPersistent>();

        public RabbitMQPersistentTests()
        {
            options.Setup(x => x.Value).Returns(new AppConfiguration
            {
                Host = "localhost",
                Password = "guest",
                UserName = "guest"
            });
            rabbitMQPersistent = new Mock<RabbitMQPersistent>(options.Object);
        }

        [Test()]
        public void RabbitMQPersistentTest()
        {
            Assert.IsFalse(rabbitMQPersistent.Object.IsConnected);
        }

        [Test()]
        public void DisposeTest()
        {

        }
    }
}