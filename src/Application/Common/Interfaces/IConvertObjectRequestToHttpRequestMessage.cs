using ASyncFramework.Domain.Model.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ASyncFramework.Application.Common.Interfaces
{
    public interface IConvertObjectRequestToHttpRequestMessage
    {
        HttpRequestMessage Convert(Request request);
        HttpRequestMessage Convert(PushRequest request, System.Collections.Generic.Dictionary<string, string> headers);
    }
}
