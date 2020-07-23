using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Request;
using System.Net.Http;
using System.Text;

namespace Subscriber.HttpRequestCode
{
    public class ConvertRequestToHttpRequestMessage : IConvertRequestToHttpRequestMessage
    {     
        public HttpRequestMessage Convert(Request request)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new System.Net.Http.HttpMethod(request.MethodVerb.ToString()), request.Url);            
            httpRequestMessage.Content = request.ContentBody==null ? null : new StringContent(request.ContentBody, Encoding.UTF8, request.ContentType);
           
            if (request.ServiceType==ASyncFramework.Domain.Enums.ServiceType.SOAP)
            {
                httpRequestMessage.Headers.Add("soapAction", request.SoapAction);
            }
            foreach (var header in request.Headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }
           
            return httpRequestMessage;
        }

    }
}
