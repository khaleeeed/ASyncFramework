using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Model.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ASyncFramework.Application.SubscribeRequestLogic.Helper
{
    public class ConvertObjectRequestToHttpRequstMessage:IConvertObjectRequestToHttpRequestMessage
    {
        public HttpRequestMessage Convert(Request request)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod(request.MethodVerb.ToString()), request.Url);
            httpRequestMessage.Content = request.ContentBody == null ? null : new StringContent(System.Convert.ToString(request.ContentBody), Encoding.UTF8, request.ContentType);

            if (request.ServiceType == ServiceType.SOAP)
                httpRequestMessage.Headers.Add("soapAction", request.SoapAction);


            if (request.Headers != null)
                foreach (var header in request.Headers)
                {
                    if (!UnregisterHeader.UnregisteredHeaders.Contains(header.Key) && !httpRequestMessage.Headers.Contains(header.Key))
                        httpRequestMessage.Headers.Add(header.Key, header.Value);
                }

            return httpRequestMessage;
        }

        public HttpRequestMessage Convert(PushRequest request, Dictionary<string, string> headers)
        {
            return Convert(new Request
            {
                Headers = headers,
                ContentBody = request.ContentBody,
                ContentType = request.ContentType,
                MethodVerb = request.MethodVerb,
                ServiceType = request.ServiceType,
                SoapAction = request.SoapAction,
                Url = request.Url
            });
        }
    }
}
