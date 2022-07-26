using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IInfrastructureLogger<T>
    {
        void LogReceived(DateTime CreationDate, bool IsCallBackMessage, string refranceNumber);
        void LogPublishing(DateTime CreationDate, Message message);
        void LogNewRequest(DateTime CreationDate, string systemId, string referenceNumber, object headrs, string targetUrl, string callBackUrl, string Content, string ContentType);
        void LogSendRequest(DateTime CreationDate, bool IsCallBackMessage, string refrenceNumber, int statusCode, string content,string url);
        void LogFinish(DateTime CreationDate, MessageLifeCycle finshMessage, string referenceNumber);
        void LogRetry(DateTime CreationDate, MessageLifeCycle retryTarget, string referenceNumber, string queue, int retry);
        void LogErrorSendRequest(DateTime CreationDate, Exception ex, bool IsCallBackMessage, string refrenceNumber,string url);
        void LogError(DateTime CreationDate, Exception ex, MessageLifeCycle lifeCycle, string referenceNumber);
        void LogError(Exception ex, string template, params object[] args);
        void LogError(string template, params object[] args);
        void LogWarning(string template,params object[] args);
    }
}