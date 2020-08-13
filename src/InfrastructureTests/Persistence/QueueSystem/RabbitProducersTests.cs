using NUnit.Framework;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Moq;
using RabbitMQ.Client;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.Tests
{
    [TestFixture()]
    public class RabbitProducersTests
    {
        Mock<IRabbitMQPersistent> mockRabbitMQPersistent = new Mock<IRabbitMQPersistent>();
        public RabbitProducersTests()
        {
            var mockIModel = new Mock<IModel>();
            mockIModel.Setup(x => x.CreateBasicProperties()).Returns(new Mock<IBasicProperties>().Object);
            mockRabbitMQPersistent.Setup(x => x.Channel).Returns(mockIModel.Object);
        }
        [Test()]
        public void RabbitProducersTest()
        {
            //_ = new RabbitProducers(mockRabbitMQPersistent.Object);
        }

        [Test()]
        public void PushMessageTest()
        {
            //RabbitProducers rabbitProducers = new RabbitProducers(mockRabbitMQPersistent.Object);
            //rabbitProducers.PushMessage(new Domain.Model.Message { }, new Domain.Common.QueueConfiguration { });
        }

        [Test()]
        public void DisposeTest()
        {
            //RabbitProducers rabbitProducers = new RabbitProducers(mockRabbitMQPersistent.Object);
            //rabbitProducers.Dispose();
            //rabbitProducers.Dispose();
        }
    }
}