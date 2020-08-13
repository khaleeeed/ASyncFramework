using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Model;
using System;
using System.Net;

namespace ASyncFramework.Domain.Interface
{
    public interface IElkLogger<T>
    {
        void LogReceived(bool IsCallBackMessage, string message, string refranceNumber);
        void LogPublishing(Message message);
        void LogNewRequest(string requestName, string systemId, object request, string referenceNumber, object headrs);
        void SendRequest(bool IsCallBackMessage, string refrenceNumber, string token, string statusCode, string content);
        void LogFinish(MessageLifeCycle finshMessage, string referenceNumber);
        void LogRetry(MessageLifeCycle retryTarget, string referenceNumber, string queue, int retry);
        void LogError(Exception ex, string template, params object[] args);
        void LogError(string template, params object[] args);
        void LogWarning(string template,params object[] args);
    }
}