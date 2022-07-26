using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Domain.Model.Response;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Logic
{
    public class PushRequestLogic : IPushRequestLogic
    {
        private readonly IRabbitProducers _rabbitProducers;
        private readonly IReferenceNumberService _referenceNumber;
        private readonly IQueueConfigurationService _queueConfiguration;
        private readonly IAllHeadersPerRequest _allHeaders;
        private readonly ICurrentUserService _currentUserService;
        public PushRequestLogic(
          IRabbitProducers rabbitProducers,
          IQueueConfigurationService queueConfiguration,
          IReferenceNumberService referenceNumber=null,
            IAllHeadersPerRequest allHeaders=null,
            ICurrentUserService currentUserService=null)
        {
            _rabbitProducers = rabbitProducers;
            _referenceNumber = referenceNumber;
            _queueConfiguration = queueConfiguration;
            _allHeaders = allHeaders;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// method use for push new request that come form user 
        /// </summary>
        public Task<Result> Push(PushRequestCommand request)
        {
            QueueConfigurations queueConfiguration = _queueConfiguration.QueueConfiguration[((IEnumerable<string>)request.Queues.Split(",", StringSplitOptions.None)).First()];

            Message message = new Message()
            {
                CallBackRequest = request.CallBackRequest,
                TargetRequest = request.TargetRequest.TargetServiceRequest,
                Queues = request.Queues,
                Retry = queueConfiguration.QueueRetry,
                ReferenceNumber = _referenceNumber.ReferenceNumber,
                Headers = _allHeaders.Headrs,
                SystemCode=_currentUserService.SystemCode,
                CallBackQueues=request.Queues,
                ExtraInfo=request.ExtraInfo,
                ServiceCode=_currentUserService.ServiceCode,
                HasCustomQueue=request.HasCustomQueue        
            };

            _rabbitProducers.PushMessage(message, queueConfiguration);
            
            return Task.FromResult(new Result(true, null)
            {
                ReferenceNumber = _referenceNumber.ReferenceNumber
            });
        }

        /// <summary>
        /// method use for push message from ASyncFramwerork like retry , push call back and retry call back
        /// </summary>
        public Task<Result> Push(Message message)
        {
            QueueConfigurations queueConfiguration = _queueConfiguration.QueueConfiguration[((IEnumerable<string>)message.Queues.Split(",")).First()];
            
            _rabbitProducers.PushMessage(message, queueConfiguration);

            return Task.FromResult(new Result(true, null)
            {
                ReferenceNumber = message.ReferenceNumber
            });
        }
    }
}