// Decompiled with JetBrains decompiler
// Type: ASyncFramework.Application.PushRequestLogic.PushRequestLogic
// Assembly: ASyncFramework.Application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3082E934-3D70-4849-9FF4-651D8F87D03C
// Assembly location: D:\Repo\ASyncFramework\src\Application\bin\Debug\netcoreapp3.1\ASyncFramework.Application.dll

using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASyncFramework.Application.PushRequestLogic
{
    public class PushRequestLogic : IPushRequestLogic
    {
        private readonly IRabbitProducers _rabbitProducers;
        private readonly IReferenceNumberService _referenceNumber;
        private readonly Dictionary<string, QueueConfiguration> _queueConfiguration;

        public PushRequestLogic(
          IRabbitProducers rabbitProducers,
          IReferenceNumberService referenceNumber,
          IOptions<Dictionary<string, QueueConfiguration>> queueConfiguration)
        {
            _rabbitProducers = rabbitProducers;
            _referenceNumber = referenceNumber;
            _queueConfiguration = queueConfiguration.Value;
        }

        public Task<Result> Push(PushRequest request)
        {
            QueueConfiguration queueConfiguration = _queueConfiguration[((IEnumerable<string>)request.Queue.Split(",", StringSplitOptions.None)).First()];
            Message message = new Message()
            {
                CallBackUri = request.CallBackUri,
                ContentBody = System.Text.Json.JsonSerializer.Serialize(request.ContentBody),
                OAuthHttpCode = request.OAuthHttpCode,
                Queue = request.Queue,
                Retry = queueConfiguration.QueueRetry,
                TargetUrl = request.TargetUrl,
                TargetVerb = request.TargetVerb,
                RefranceNumber = _referenceNumber.ReferenceNumber,
                OAuthHttpCodeCallBack=request.CallBackOAuthHttpCode,
                             
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
            QueueConfiguration queueConfiguration = _queueConfiguration[((IEnumerable<string>)message.Queue.Split(",", StringSplitOptions.None)).First()];
            using (_rabbitProducers)
                _rabbitProducers.PushMessage(message, queueConfiguration);
            return Task.FromResult(new Result(true, null)
            {
                ReferenceNumber = _referenceNumber.ReferenceNumber
            });
        }
    }
}
