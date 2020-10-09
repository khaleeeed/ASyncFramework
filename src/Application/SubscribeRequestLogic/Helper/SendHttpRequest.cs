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
        private string _token;
        private readonly IConvertObjectRequestToHttpRequestMessage _convertFromRequestToHttpRequestMessage;
        private readonly IElkLogger<SendHttpRequest> _logger;

        public SendHttpRequest(IConvertObjectRequestToHttpRequestMessage convertFromRequestToHttpRequestMessage, IElkLogger<SendHttpRequest> logger)
        {
            _convertFromRequestToHttpRequestMessage = convertFromRequestToHttpRequestMessage;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> SendRequest(Message message, Task<Task> taskGetToken)
        {
            // create request 
            using HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using HttpClient client = new HttpClient(clientHandler);

            // convert request object to httpRequestMessage
            using HttpRequestMessage httpRequestMessage = _convertFromRequestToHttpRequestMessage.Convert(message.TargetRequest, message.Headers);

            // wait token 
            if (taskGetToken != null)
            {
                // double await for (wait convert request object to httpRequestMessage) and (wait get token request from GetToken method)
                await await taskGetToken;
                // set token header 
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }

            try
            {
                // send request 
                var httpResponseMessage = await client.SendAsync(httpRequestMessage);

                _logger.LogSendRequest(message.IsCallBackMessage, message.ReferenceNumber, _token, Convert.ToInt32(httpResponseMessage.StatusCode), await httpResponseMessage.Content?.ReadAsStringAsync(), message.TargetRequest.Url);
                return httpResponseMessage;

            }
            catch (Exception ex)
            {
                _logger.LogErrorSendRequest(ex, message.IsCallBackMessage, message.ReferenceNumber, message.TargetRequest.Url);
                return null;
            }

        }

        // call OAuht request and set _token property
        public async Task GetToken(Task<HttpRequestMessage> taskHttpRequestMessage)
        {
            using HttpRequestMessage httpRequestMessage = await taskHttpRequestMessage;
            using HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using HttpClient client = new HttpClient(clientHandler);
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            _token = System.Text.Json.JsonSerializer.Deserialize<AuthModel>(content, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).token;
        }
    }
}
