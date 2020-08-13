// Decompiled with JetBrains decompiler
// Type: ASyncFramework.Application.PushRequestLogic.PushRequestLogic
// Assembly: ASyncFramework.Application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3082E934-3D70-4849-9FF4-651D8F87D03C
// Assembly location: D:\Repo\ASyncFramework\src\Application\bin\Debug\netcoreapp3.1\ASyncFramework.Application.dll

using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ASyncFramework.Application.PushRequestLogic
{
    public class PushRequestLogic : IPushRequestLogic
    {
        private readonly IRabbitProducers _rabbitProducers;
        private readonly IReferenceNumberService _referenceNumber;
        private readonly Dictionary<string, QueueConfiguration> _queueConfiguration;
        private readonly IAllHeadersPerRequest _allHeaders;

        public PushRequestLogic(
          IRabbitProducers rabbitProducers,
          IReferenceNumberService referenceNumber,
          IOptions<Dictionary<string, QueueConfiguration>> queueConfiguration,
            IAllHeadersPerRequest allHeaders)
        {
            _rabbitProducers = rabbitProducers;
            _referenceNumber = referenceNumber;
            _queueConfiguration = queueConfiguration.Value;
            _allHeaders = allHeaders;
        }


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
            using (_rabbitProducers)            
                _rabbitProducers.PushMessage(message, queueConfiguration);
            
            return Task.FromResult(new Result(true, null)
            {
                ReferenceNumber = _referenceNumber.ReferenceNumber
            });
        }

        public Task<Result> Push(Message message)
        {
            QueueConfiguration queueConfiguration = _queueConfiguration[((IEnumerable<string>)message.Queues.Split(",")).First()];
            using (_rabbitProducers)
                _rabbitProducers.PushMessage(message, queueConfiguration);

            return Task.FromResult(new Result(true, null)
            {
                ReferenceNumber = message.ReferenceNumber
            });
        }
    }
}
