using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Model;
using System;
using System.Net;

namespace ASyncFramework.Domain.Interface
{
    public interface IElkLogger<T>
    {
        void LogReceived(bool IsCallBackMessage, string refranceNumber);
        void LogPublishing(Message message);
        void LogNewRequest(string systemId, object request, string referenceNumber, object headrs, string hash, string targetUrl, string callBackUrl,string content,string ContentType);
        void LogSendRequest(bool IsCallBackMessage, string refrenceNumber, string token, int statusCode, string content,string url);
        void LogFinish(MessageLifeCycle finshMessage, string referenceNumber);
        void LogRetry(MessageLifeCycle retryTarget, string referenceNumber, string queue, int retry);
        void LogErrorSendRequest(Exception ex, bool IsCallBackMessage, string refrenceNumber,string url);
        void LogError(Exception ex, MessageLifeCycle lifeCycle, string referenceNumber);
        void LogError(Exception ex, string template, params object[] args);
        void LogError(string template, params object[] args);
        void LogWarning(string template,params object[] args);
    }
}