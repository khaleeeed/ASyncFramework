using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace ASyncFramework.Infrastructure.Persistence.LoggingRepo
{

    // register as Singleton
    // responsibility log in elk 
    public class ElkLogger<T> : IElkLogger<T>
    {

        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public ElkLogger(ILogger<T> logger)
        {
            _logger = logger;
        }


        public void LogNewRequest(string systemId, object request, string referenceNumber, object headrs, string hash, string targetUrl, string callBackUrl, string Content, string ContentType)
        {

            var obj = ObjectConverter.ContentType(Content, ContentType);

            Log.ForContext("ContentBody", obj, true).ForContext("Headers", headrs, true).Information(
                "{CreationDate} {Status} {UserId} {Request} {ReferenceNumber} {Hash} {TargetUrl} {CallBackUrl}",
                        DateTime.Now, MessageLifeCycle.NewRequest, systemId,
                        Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented),
                         referenceNumber, hash, targetUrl, callBackUrl);
        }

        public void LogPublishing(Message message)
        {
            Log.ForContext("Message", message, true).Information("{CreationDate} {Status} {ReferenceNumber}",
               DateTime.Now, message.IsCallBackMessage ? MessageLifeCycle.PushCallBackToQueue : MessageLifeCycle.PushToQueue, message.ReferenceNumber);
        }

        public void LogReceived(bool IsCallBackMessage, string refranceNumber)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber}",
                        DateTime.Now, IsCallBackMessage ? MessageLifeCycle.ReceiveCallBackFromQueue : MessageLifeCycle.ReceiveFromQueue,
                        refranceNumber);
        }

        public void LogSendRequest(bool IsCallBackMessage, string refrenceNumber, string token, int statusCode, string content, string url)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber} {Token} {StatusCode} {ResponseContent} {Url}",
                DateTime.Now, IsCallBackMessage ? MessageLifeCycle.SendRequestToCallBack : MessageLifeCycle.SendRequestToTarget,
                refrenceNumber, token, statusCode, content, url);
        }

        public void LogFinish(MessageLifeCycle finshMessage, string referenceNumber)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber}",
                        DateTime.Now, finshMessage, referenceNumber);
        }

        public void LogRetry(MessageLifeCycle retryTarget, string referenceNumber, string queue, int retry)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber} {Queues} {Retry}",
                DateTime.Now, retryTarget, referenceNumber, queue, retry);
        }

        public void LogErrorSendRequest(Exception ex, bool IsCallBackMessage, string refrenceNumber, string url)
        {
            _logger.LogError(ex, "{CreationDate} {Status} {ReferenceNumber} {Url}",
                DateTime.Now, IsCallBackMessage ? MessageLifeCycle.ExceptoinWhenSendCallBackRequest : MessageLifeCycle.ExceptoinWhenSendTargetRequest,
                refrenceNumber, url);
        }

        public void LogError(Exception ex, MessageLifeCycle lifeCycle, string referenceNumber)
        {
            _logger.LogError(ex, "{CreationDate} {Status} {ReferenceNumber}", DateTime.Now, lifeCycle, referenceNumber);
        }

        public void LogError(Exception ex, string template, params object[] args)
        {
            _logger.LogError(ex, template, args);
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
