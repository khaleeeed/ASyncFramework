using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Application.SubscribeRequestLogic.Helper
{
    public class SendHttpRequest:ISendHttpRequest
    {
        private readonly IConvertObjectRequestToHttpRequestMessage _convertFromRequestToHttpRequestMessage;
        private readonly IInfrastructureLogger<SendHttpRequest> _logger;

        public SendHttpRequest(IConvertObjectRequestToHttpRequestMessage convertFromRequestToHttpRequestMessage, IInfrastructureLogger<SendHttpRequest> logger)
        {
            _convertFromRequestToHttpRequestMessage = convertFromRequestToHttpRequestMessage;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> SendRequest(Message message)
        {
            // create request 
            using HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.Brotli
            };
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using HttpClient client = new HttpClient(clientHandler);
            client.Timeout = TimeSpan.FromMinutes(3);
            // convert request object to httpRequestMessage
            using HttpRequestMessage httpRequestMessage = _convertFromRequestToHttpRequestMessage.Convert(message.TargetRequest, message.Headers);

            try
            {
                // send request 
                var httpResponseMessage = await client.SendAsync(httpRequestMessage);

                _logger.LogSendRequest(DateTime.Now,message.IsCallBackMessage, message.ReferenceNumber, Convert.ToInt32(httpResponseMessage.StatusCode), await httpResponseMessage.Content?.ReadAsStringAsync(), message.TargetRequest.Url);
                return httpResponseMessage;

            }
            catch (Exception ex)
            {
                _logger.LogErrorSendRequest(DateTime.Now,ex, message.IsCallBackMessage, message.ReferenceNumber, message.TargetRequest.Url);
                return null;
            }

        }
        
    }
}
