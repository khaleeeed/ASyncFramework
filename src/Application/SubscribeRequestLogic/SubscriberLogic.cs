using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Application.SubscribeRequestLogic
{
    public class SubscriberLogic : ISubscriberLogic
    {
        private string _token;
        private readonly IConvertRequestToHttpRequestMessage _convertFromCodeHttpToObject;
        private readonly IPushRequestLogic _pushRequestLogic;
        private readonly IOptions<Dictionary<string, QueueConfiguration>> _queueConfiguration;

        public SubscriberLogic(IConvertRequestToHttpRequestMessage convertFromCodeHttpToObject, IPushRequestLogic pushRequestLogic, IOptions<Dictionary<string, QueueConfiguration>> queueConfiguration)
        {
            _convertFromCodeHttpToObject = convertFromCodeHttpToObject;
            _pushRequestLogic = pushRequestLogic;
            _queueConfiguration = queueConfiguration;
        }

        public async Task Subscribe(Message message)
        {

            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
            Task<Task> taskGetToken = null;
            
            //check if the request have token 
            if (message.TargetOAuthRequest != null)
            {
                // get httpRequest Message 
                var taskHttpRequestMessage = Task.Run(() => _convertFromCodeHttpToObject.Convert(message.TargetOAuthRequest));
                // get token 
                taskGetToken=taskHttpRequestMessage.ContinueWith(GetToken);
            }
            // send request 
            var httpResponseMessage = await SendRequest(message, taskGetToken);

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
                _ = Retry(message,true);
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
                _ = Retry(message);
            }
        }

        private Task Retry(Message message,bool isMessageCallBack=false)
        {
            // if there no retry in queue push to next long queue 
            if (message.Retry <= 0)
            {
                var Queues = message.Queues.Split(',');
                // messageCallBack push message to call back fauiler queues 
                if (Queues.Length < 2 && isMessageCallBack==true)
                {
                    var queueConfig= _queueConfiguration.Value["CallBackFailuer"];
                    message.Queues = queueConfig.QueueName;
                    message.Retry = queueConfig.QueueRetry;
                    _ = _pushRequestLogic.Push(message);
                    return Task.CompletedTask;
                }
                // check if there next long queue else request will lost 
                if (Queues.Length < 2)
                {
                    return Task.CompletedTask;
                }
                message.Queues = Queues.Skip(1).Aggregate((x, y) => $"{x},{y}");
                message.Retry = _queueConfiguration.Value[Queues.Skip(1).FirstOrDefault()].QueueRetry + 1;
            }
            message.Retry -= 1;
            // retry
            _ = _pushRequestLogic.Push(message);
            return Task.CompletedTask;
        }

        private async Task PushForCallBackApi(Message message, HttpResponseMessage httpResponseMessage)
        {
            var queues = _queueConfiguration.Value.Keys.Select(x => x).Aggregate((x, y) => $"{x},{y}");
            var conent = await httpResponseMessage.Content?.ReadAsStringAsync();
            var headers=httpResponseMessage.Headers?.ToDictionary(k => k.Key, k => k.Value.Aggregate((x,y)=>x+y));
            _ = _pushRequestLogic.Push(new Message
            {
                TargetRequest= new Domain.Model.Request.PushRequest 
                { 
                    ContentBody=conent,
                    Url=message.CallBackRequest.Url,
                    ContentType=message.CallBackRequest.ContentType,
                    MethodVerb=Domain.Enums.MethodVerb.Post,
                    ServiceType=message.CallBackRequest.ServiceType,
                    SoapAction=message.CallBackRequest.SoapAction
                } ,
                TargetOAuthRequest=message.CallBackOAuthRequest,
                IsCallBackMessage = true,
                Queues = queues,
                ReferenceNumber = message.ReferenceNumber,
                Retry = _queueConfiguration.Value.Values.First().QueueRetry,
                HttpStatusCode = httpResponseMessage.StatusCode.ToString(),
                Headers=headers
                
            });
        }

        private async Task<HttpResponseMessage> SendRequest(Message message, Task<Task> taskGetToken)
        {
            using HttpClient client = new HttpClient();
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod(message.TargetRequest.MethodVerb.ToString()), message.TargetRequest.Url);
            httpRequestMessage.Content = message.TargetRequest.ContentBody == null ? null : new StringContent(message.TargetRequest.ContentBody, Encoding.UTF8, message.TargetRequest.ContentType);
            client.DefaultRequestHeaders.Add("ASyncCallHttpStatucCode", message.HttpStatusCode);
            if (message.Headers != null)
                foreach (var header in message.Headers)
                {
                    if (!UnregisterHeader.UnregisteredHeaders.Contains(header.Key) && !httpRequestMessage.Headers.Contains(header.Key))
                        httpRequestMessage.Headers.Add(header.Key, header.Value);
                }

            // wait token 
            if (taskGetToken != null)
                await await taskGetToken;
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            if (message.TargetRequest.ServiceType == Domain.Enums.ServiceType.SOAP)
                client.DefaultRequestHeaders.Add("soapAction", message.TargetRequest.SoapAction);
            return await client.SendAsync(httpRequestMessage);
        }

        private async Task GetToken(Task<HttpRequestMessage> taskHttpRequestMessage)
        {
            HttpRequestMessage httpRequestMessage = await taskHttpRequestMessage;            
            using (HttpClient client = new HttpClient())
            {
                var httpResponseMessage = await client.SendAsync(httpRequestMessage);
                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                _token = System.Text.Json.JsonSerializer.Deserialize<AuthModel>(content, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).token;
            }
        }
    }
}