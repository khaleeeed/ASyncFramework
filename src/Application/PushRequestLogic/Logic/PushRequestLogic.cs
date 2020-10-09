using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
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
        private readonly Dictionary<string, QueueConfiguration> _queueConfiguration;
        private readonly IAllHeadersPerRequest _allHeaders;

        public PushRequestLogic(
          IRabbitProducers rabbitProducers,
          IOptions<Dictionary<string, QueueConfiguration>> queueConfiguration,
          IReferenceNumberService referenceNumber=null,
            IAllHeadersPerRequest allHeaders=null)
        {
            _rabbitProducers = rabbitProducers;
            _referenceNumber = referenceNumber;
            _queueConfiguration = queueConfiguration.Value;
            _allHeaders = allHeaders;
        }

        /// <summary>
        /// method use for push new request that come form user 
        /// </summary>
        public Task<Result> Push(PushRequestCommand request)
        {
            QueueConfiguration queueConfiguration = _queueConfiguration[((IEnumerable<string>)request.Queues.Split(",", StringSplitOptions.None)).First()];

            Message message = new Message()
            {
                CallBackOAuthRequest = request.CallBackOAuthRequest,
                CallBackRequest = request.CallBackRequest,
                TargetOAuthRequest = request.TargetOAuthRequest,
                TargetRequest = request.TargetRequest,
                Queues = request.Queues,
                Retry = queueConfiguration.QueueRetry,
                ReferenceNumber = _referenceNumber.ReferenceNumber,
                Headers = _allHeaders.Headrs
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
            QueueConfiguration queueConfiguration = _queueConfiguration[((IEnumerable<string>)message.Queues.Split(",")).First()];
            
            _rabbitProducers.PushMessage(message, queueConfiguration);

            return Task.FromResult(new Result(true, null)
            {
                ReferenceNumber = message.ReferenceNumber
            });
        }
    }
}
