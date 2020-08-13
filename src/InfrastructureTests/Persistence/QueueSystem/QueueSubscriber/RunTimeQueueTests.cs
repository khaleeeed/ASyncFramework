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
        Mock<IOptions<Dictionary<string, QueueConfiguration>>> mockOptions = new Mock<IOptions<Dictionary<string, QueueConfiguration>>>();
        public RunTimeQueueTests()
        {
            mockOptions.Setup(x => x.Value).Returns(new Dictionary<string, QueueConfiguration> { { "0", new QueueConfiguration {  IsAutoMapping=true} }, { "1", new QueueConfiguration { } }, { "2", new QueueConfiguration {IsAutoMapping=true } } });
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