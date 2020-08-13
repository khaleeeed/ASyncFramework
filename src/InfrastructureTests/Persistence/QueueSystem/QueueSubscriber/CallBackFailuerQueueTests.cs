using NUnit.Framework;
using ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using System.Threading;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber.Tests
{
    [TestFixture()]
    public class CallBackFailuerQueueTests
    {
        Mock<IRabbitMQPersistent> mockRabbitMQPersistent = new Mock<IRabbitMQPersistent>();
        Mock<ISubscriberLogic> mockSubscriberLogic = new Mock<ISubscriberLogic>();        

        [Test()]
        public void CallBackFailuerQueueTest()
        {
            //_ = new CallBackFailuerQueue(mockRabbitMQPersistent.Object, mockSubscriberLogic.Object);
            
        }
        [Test()]
        public void ProcessTest()
        {
        //    CallBackFailuerQueue callBackFailuerQueue = new CallBackFailuerQueue(mockRabbitMQPersistent.Object, mockSubscriberLogic.Object);
        //    Assert.IsTrue(callBackFailuerQueue.Process(System.Text.Json.JsonSerializer.Serialize(new Message { HttpStatusCode="test" })).Result);
        }
        [Test()]
        public void DisposeTest()
        {
            //CallBackFailuerQueue callBackFailuerQueue = new CallBackFailuerQueue(mockRabbitMQPersistent.Object, mockSubscriberLogic.Object);
            //callBackFailuerQueue.Dispose();
            //callBackFailuerQueue.Dispose();
            //callBackFailuerQueue.Dispose();
        }
        [Test()]
        public void StartAsyncTest()
        {
            //CallBackFailuerQueue callBackFailuerQueue = new CallBackFailuerQueue(mockRabbitMQPersistent.Object, mockSubscriberLogic.Object);
            //callBackFailuerQueue.StartAsync(CancellationToken.None);
        }
        [Test()]
        public void StopAsyncTest()
        {
            //CallBackFailuerQueue callBackFailuerQueue = new CallBackFailuerQueue(mockRabbitMQPersistent.Object, mockSubscriberLogic.Object);
            //callBackFailuerQueue.StopAsync(CancellationToken.None);
        }
    }
}