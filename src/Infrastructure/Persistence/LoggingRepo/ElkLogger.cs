using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.LoggingRepo
{
    public class ElkLogger<T> : IElkLogger<T>
    {
        private ILogger _logger;

        public ElkLogger(ILogger<T> logger)
        {
            _logger = logger;
        }
    
         
        public void LogNewRequest(string requestName, string systemId, object request, string referenceNumber, object headrs)
        {
            _logger.LogInformation("{Status}: {Name} {UserId} {Request} {ReferenceNumber} {Headers}",
                        MessageLifeCycle.NewRequest, requestName, systemId, Newtonsoft.Json.JsonConvert.SerializeObject(request), referenceNumber, Newtonsoft.Json.JsonConvert.SerializeObject(headrs));
        }

        public void LogPublishing(Message message)
        {
            _logger.LogInformation("{status} {ReferenceNumber} {Message}",
               message.IsCallBackMessage ? MessageLifeCycle.PublishingCallBack : MessageLifeCycle.PublishingTarget, message.ReferenceNumber, Newtonsoft.Json.JsonConvert.SerializeObject(message));
        }

        public void LogReceived(bool IsCallBackMessage,string message,string refranceNumber)
        {
            _logger.LogInformation("{Status}: {Message} {ReferenceNumber}",
                        IsCallBackMessage ? MessageLifeCycle.ReceivedCallBack : MessageLifeCycle.ReceivedTarget,
                        message, refranceNumber);
        }

        public void SendRequest(bool IsCallBackMessage, string refrenceNumber,string token,string statusCode,string content)
        {
            _logger.LogInformation("{Status} {ReferenceNumber} {Token} {StatusCode} {ResponseContent}",
                IsCallBackMessage ? MessageLifeCycle.SendCallBackRequest: MessageLifeCycle.SendTargetRequest, refrenceNumber, token, statusCode, content);
        }

        public void LogFinish(MessageLifeCycle  finshMessage, string referenceNumber)
        {
            _logger.LogInformation("{Status}: {ReferenceNumber}",
                        finshMessage, referenceNumber);
        }

        public void LogRetry(MessageLifeCycle retryTarget, string referenceNumber, string queue, int retry)
        {
            _logger.LogInformation("{Status} {ReferenceNumber} {Queues} {Retry}",
                retryTarget, queue, queue, retry);
        }

        public void LogError(Exception ex,string template,params object[] args)
        {
            _logger.LogError(ex,template,args);
        }
        public void LogError(string template, params object[] args)
        {
            _logger.LogError(template, args);

        }
        public void LogWarning(string template, params object[] args)
        {
            _logger.LogWarning(template, args);
        }        
    }
}
