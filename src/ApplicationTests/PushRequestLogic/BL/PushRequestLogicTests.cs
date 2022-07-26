using NUnit.Framework;
using ASyncFramework.Application.PushRequestLogic;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Model;

namespace ASyncFramework.Application.PushRequestLogic.Tests
{
    [TestFixture()]
    public class PushRequestLogicTests
    {
        Mock<IRabbitProducers> mockRabbitProducers = new Mock<IRabbitProducers>();
        Mock<IReferenceNumberService> mockReferenceNumberService = new Mock<IReferenceNumberService>();
        Mock<IOptions<Dictionary<string, QueueConfigurations>>> mockOptions = new Mock<IOptions<Dictionary<string, QueueConfigurations>>>();
        Mock<IAllHeadersPerRequest> mockAllHeadersPerRequest = new Mock<IAllHeadersPerRequest>();
        public PushRequestLogicTests()
        {
            mockOptions.Setup(x => x.Value).Returns(new Dictionary<string, QueueConfigurations> { { "0", new QueueConfigurations { IsAutoMapping = true } }, { "1", new QueueConfigurations { } }, { "2", new QueueConfigurations { IsAutoMapping = true } } });
            mockReferenceNumberService.Setup(x => x.ReferenceNumber).Returns(Guid.NewGuid().ToString());
        }
        [Test()]
        public void PushTest()
        {
            //PushRequestLogic pushRequestLogic = new PushRequestLogic(mockRabbitProducers.Object, mockReferenceNumberService.Object, mockOptions.Object, mockAllHeadersPerRequest.Object);
            //var res=pushRequestLogic.Push(new PushRequestCommand { Queues="1,5,5" }).Result;
            //Assert.IsTrue(res.Succeeded);
            //Assert.IsNotEmpty(res.ReferenceNumber);
        }

        [Test()]
        public void PushMessageTest()
        {
            //PushRequestLogic pushRequestLogic = new PushRequestLogic(mockRabbitProducers.Object, mockReferenceNumberService.Object, mockOptions.Object, mockAllHeadersPerRequest.Object);
            //var res = pushRequestLogic.Push(new Message { Queues = "1,5,5" }).Result;
            //Assert.IsTrue(res.Succeeded);
            //Assert.IsNotEmpty(res.ReferenceNumber);
        }
    }
}