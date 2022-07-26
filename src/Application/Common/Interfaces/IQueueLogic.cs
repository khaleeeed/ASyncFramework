using ASyncFramework.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Common.Interfaces
{
    public interface IQueueLogic
    {
        Task Retry(Message message, int statusCode, bool isMessageCallBack = false);
        Task PushForCallBackApi(Message message, HttpResponseMessage httpResponseMessage);
        Task FailureLogic(Message message,HttpResponseMessage httpResponseMessage);
        
    }
}
