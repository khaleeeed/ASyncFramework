using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.LoggingRepo
{

    // register as Singleton
    // responsibility log in elk 
    public class ElkLogger<T> : IInfrastructureLogger<T>
    {

        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public ElkLogger(ILogger<T> logger)
        {
            _logger = logger;
        }


        public void LogNewRequest(DateTime CreationDate, string systemId, string referenceNumber, object headrs, string targetUrl, string callBackUrl, string Content, string ContentType)
        {

            var obj = ObjectConverter.ContentType(Content, ContentType);

            Log.ForContext("ContentBody", obj, true)
                .ForContext("Headers", headrs, true)
                .Information("{CreationDate} {Status} {ReferenceNumber} {TargetUrl} {CallBackUrl}",
                DateTime.Now, MessageLifeCycle.NewRequest,referenceNumber,targetUrl,callBackUrl);  
        }


        public void LogPublishing(DateTime CreationDate, Message message)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber} {Message}",
               CreationDate, message.IsCallBackMessage ? MessageLifeCycle.PushCallBackToQueue : MessageLifeCycle.PushToQueue, message.ReferenceNumber, Newtonsoft.Json.JsonConvert.SerializeObject(message));
        }

        public void LogReceived(DateTime CreationDate, bool IsCallBackMessage, string refranceNumber)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber}",
                        CreationDate, IsCallBackMessage ? MessageLifeCycle.ReceiveCallBackFromQueue : MessageLifeCycle.ReceiveFromQueue,
                        refranceNumber);
        }

        public void LogSendRequest(DateTime CreationDate, bool IsCallBackMessage, string refrenceNumber, int statusCode, string content, string url)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber} {StatusCode} {ResponseContent} {Url}",
                CreationDate, IsCallBackMessage ? MessageLifeCycle.SendRequestToCallBack : MessageLifeCycle.SendRequestToTarget,
                refrenceNumber, statusCode, content, url);
        }

        public void LogFinish(DateTime CreationDate, MessageLifeCycle finshMessage, string referenceNumber)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber}",
                        CreationDate, finshMessage, referenceNumber);

        }

        public void LogRetry(DateTime CreationDate, MessageLifeCycle retryTarget, string referenceNumber, string queue, int retry)
        {
            _logger.LogInformation("{CreationDate} {Status} {ReferenceNumber} {Queues} {Retry}",
                CreationDate, retryTarget, referenceNumber, queue, retry);
        }

        public void LogErrorSendRequest(DateTime CreationDate, Exception ex, bool IsCallBackMessage, string refrenceNumber, string url)
        {
            _logger.LogError(ex, "{CreationDate} {Status} {ReferenceNumber} {Url}",
                CreationDate, IsCallBackMessage ? MessageLifeCycle.ExceptoinWhenSendCallBackRequest : MessageLifeCycle.ExceptoinWhenSendTargetRequest,
                refrenceNumber, url);
        }

        public void LogError(DateTime CreationDate, Exception ex, MessageLifeCycle lifeCycle, string referenceNumber)
        {
            _logger.LogError(ex, "{CreationDate} {Status} {ReferenceNumber}", CreationDate, lifeCycle, referenceNumber);
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
