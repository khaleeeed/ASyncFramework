using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Application.SubscribeRequestLogic
{
    public class SubscriberLogic
    {
        private string _token;
        private readonly IConvertFromCodeHttpToObject _convertFromCodeHttpToObject;
        private readonly IPushRequestLogic _pushRequestLogic;
        private readonly IOptions<Dictionary<string, QueueConfiguration>> _queueConfiguration;

        public SubscriberLogic(IConvertFromCodeHttpToObject convertFromCodeHttpToObject, IPushRequestLogic pushRequestLogic, IOptions<Dictionary<string, QueueConfiguration>> queueConfiguration)
        {
            _convertFromCodeHttpToObject = convertFromCodeHttpToObject;
            _pushRequestLogic = pushRequestLogic;
            _queueConfiguration = queueConfiguration;
        }

        public async Task Subscribe(Message message)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            // get httpRequest Message 
            var taskHttpRequestMessage = Task.Run(() => _convertFromCodeHttpToObject.Convert(message.OAuthHttpCode));
            // get token 
            await taskHttpRequestMessage.ContinueWith(GetToken);
            var awaiter = taskHttpRequestMessage.GetAwaiter();

            // send request 
            var httpResponseMessage= await SendRequest(message, taskHttpRequestMessage);

            // if call Back message 
            if (message.IsCallBackMessage)
            {
                // if call back message success stop function 
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return;         
                }
                // if call back message not success retry 
                // retry 
                _ = Retry(message);
            }

            // success or user error push message to queue for call  callbackService 
            if (httpResponseMessage.IsSuccessStatusCode || httpResponseMessage.StatusCode < System.Net.HttpStatusCode.InternalServerError)
            {
                await PushForCallBackApi(message, httpResponseMessage);
                return;
            }


            // service error retry 
            if (httpResponseMessage.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
            {
                // retry 
                _=Retry(message);
            }
        }

        private Task Retry(Message message)
        {
            // if there no retry in queue push to next long queue 
            if (message.Retry <= 0)
            {
                var Queues = message.Queue.Split(',');
                // check if there next long queue else request will lost 
                if (Queues.Length < 2)
                {
                    return Task.CompletedTask;
                }
                message.Queue = Queues.Skip(1).Aggregate((x, y) => $"{x},{y}");
                message.Retry = _queueConfiguration.Value[Queues.Skip(1).FirstOrDefault()].QueueRetry + 1;
            }
            message.Retry -= 1;
            // retry
            _ = _pushRequestLogic.Push(message);
            return Task.CompletedTask;
        }

        private async Task PushForCallBackApi(Message message, HttpResponseMessage httpResponseMessage)
        {
            var queue = _queueConfiguration.Value.Keys.Select(x => x).Aggregate((x, y) => $"{x},{y}");
            var conent = await httpResponseMessage.Content?.ReadAsStringAsync();
            _ = _pushRequestLogic.Push(new Message
            {
                TargetUrl = message.CallBackUri,
                ContentBody = conent,
                IsCallBackMessage = true,
                Queue = queue,
                OAuthHttpCode = message.OAuthHttpCodeCallBack,
                RefranceNumber = message.RefranceNumber,
                TargetVerb = Domain.Enums.TargetVerb.Post,
                Retry = _queueConfiguration.Value.Values.First().QueueRetry,
                HttpStatusCode = httpResponseMessage.StatusCode.ToString()
            });
        }

        private async Task<HttpResponseMessage> SendRequest(Message message, Task<HttpRequestMessage> taskHttpRequestMessage)
        {
            using HttpClient client = new HttpClient();
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod(message.TargetVerb.ToString()), message.TargetUrl);
            httpRequestMessage.Content = new StringContent(message.ContentBody, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("ASyncCallHttpStatucCode",message.HttpStatusCode);
            foreach (var header in message.Headers)
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            // wait token 
            var res = await taskHttpRequestMessage;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return await client.SendAsync(httpRequestMessage);
        }

        private async Task GetToken (Task<HttpRequestMessage> taskHttpRequestMessage)
        {
            HttpRequestMessage httpRequestMessage = await taskHttpRequestMessage;
            using HttpClient client = new HttpClient();
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            _token = System.Text.Json.JsonSerializer.Deserialize<AuthModel>(content).accessToken;
        }
    }
}
