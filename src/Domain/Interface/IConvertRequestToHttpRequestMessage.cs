using ASyncFramework.Domain.Model.Request;
using System.Net.Http;

namespace ASyncFramework.Domain.Interface
{
    public interface IConvertRequestToHttpRequestMessage
    {
        HttpRequestMessage Convert(Request codeHttp);
    }
}