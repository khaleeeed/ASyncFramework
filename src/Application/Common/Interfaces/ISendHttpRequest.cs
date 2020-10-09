using ASyncFramework.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Common.Interfaces
{
    public interface ISendHttpRequest
    {
        Task<HttpResponseMessage> SendRequest(Message message, Task<Task> taskGetToken);
        Task GetToken(Task<HttpRequestMessage> taskHttpRequestMessage);

    }
}
