using NUnit.Framework;
using System;
using System.Collections.Generic;
using Moq;
using ASyncFramework.Domain.Common;
using Microsoft.Extensions.Options;
using System.Threading;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber.Tests
{
    [TestFixture()]
    public class RunTimeQueueTests
    {
        Mock<IServiceProvider> mockServiceProvider = new Mock<IServiceProvider>();
        Mock<IOptions<Dictionary<string, QueueConfigurations>>> mockOptions = new Mock<IOptions<Dictionary<string, QueueConfigurations>>>();
        public RunTimeQueueTests()
        {
            mockOptions.Setup(x => x.Value).Returns(new Dictionary<string, QueueConfigurations> { { "0", new QueueConfigurations {  IsAutoMapping=true} }, { "1", new QueueConfigurations { } }, { "2", new QueueConfigurations {IsAutoMapping=true } } });
        }
        [Test()]
        public void RunTimeQueueTest()
        {
            //_ = new RunTimeQueue(mockServiceProvider.Object, mockOptions.Object);
        }

        [Test()]
        public void StartAsyncTest()
        {
            //var RunTimeQueue = new RunTimeQueue(mockServiceProvider.Object, mockOptions.Object);
            //RunTimeQueue.StartAsync(CancellationToken.None);
        }

        [Test()]
        public void StopAsyncTest()
        {
            //var RunTimeQueue = new RunTimeQueue(mockServiceProvider.Object, mockOptions.Object);
            //RunTimeQueue.StopAsync(CancellationToken.None);
        }
    }
}