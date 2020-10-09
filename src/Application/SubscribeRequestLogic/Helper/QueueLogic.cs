using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.Logic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ASyncFramework.Application.SubscribeRequestLogic.Helper
{
    public class QueueLogic: IQueueLogic
    {
        private readonly IElkLogger<QueueLogic> _logger;
        private readonly ICallBackFailureRepository _CallBackFailureRepository;
        private readonly ITargetFailuerRepository _TargetFailuerRepository;
        private readonly IOptions<Dictionary<string, QueueConfiguration>> _queueConfiguration;
        private readonly IPushRequestLogic _pushRequestLogic;

        public QueueLogic(IElkLogger<QueueLogic> logger, ICallBackFailureRepository callBackFailureRepository, ITargetFailuerRepository targetFailuerRepository, IOptions<Dictionary<string, QueueConfiguration>> queueConfiguration, IPushRequestLogic pushRequestLogic)
        {
            _logger = logger;
            _CallBackFailureRepository = callBackFailureRepository;
            _TargetFailuerRepository = targetFailuerRepository;
            _queueConfiguration = queueConfiguration;
            _pushRequestLogic = pushRequestLogic;
        }

        public Task Retry(Message message, int statusCode, bool isMessageCallBack = false)
        {
            // check if queue UsedAllRetries
            if (message.Retry <= 0)
            {
                var Queues = message.Queues.Split(',');

                // check if there next queue available and message call back
                if (Queues.Length < 2)
                {
                    if (isMessageCallBack == true)
                    {
                        // get CallBackFailuer configuration and push message to call back fauiler 
                        _logger.LogFinish(MessageLifeCycle.CallBackUsedAllRetries, message.ReferenceNumber);
                        // add call back fauiler to elk index 
                        _ = _CallBackFailureRepository.Add(new Domain.Documents.CallBackFailuerDocument
                        {
                            Fields = new Domain.Documents.CallBackFields
                            {
                                CallBackUrl = message.TargetRequest.Url,
                                IsSendSuccessfully = false,
                                ReferenceNumber = message.ReferenceNumber,
                                ResponseContent = message.TargetRequest.ContentBody,
                                StatusCode = statusCode,
                                CreationDate = DateTime.Now,
                                Retry = 0,
                                SystemCode = message.SystemCode,
                                IsProcessing=false,
                                Message=System.Text.Json.JsonSerializer.Serialize(message)
                            }
                        });
                        return Task.CompletedTask;
                    }

                    // if there no next queue available and message in target request. message will lost 
                    _logger.LogFinish(MessageLifeCycle.TargetUsedAllRetries, message.ReferenceNumber);
                    // add taget faiuler to elk index 
                    _ = _TargetFailuerRepository.Add(new Domain.Documents.TargetFailuerDocument
                    {
                        Fields = new Domain.Documents.TargetFields
                        {
                            CallBackUrl = message.CallBackRequest.Url,
                            ContentBody = message.TargetRequest.ContentBody,
                            IsSendSuccessfully = false,
                            Method = message.TargetRequest.MethodVerb.ToString(),
                            ReferenceNumber = message.ReferenceNumber,
                            CreationDate = DateTime.Now,
                            Retry = 0,
                            StatusCode = statusCode,
                            SystemCode=message.SystemCode,
                            IsProcessing = false,
                            Message = System.Text.Json.JsonSerializer.Serialize(message)
                        }
                    });
                    return Task.CompletedTask;
                }

                // set all another queue and skip queue UsedAllRetries
                message.Queues = Queues.Skip(1).Aggregate((x, y) => $"{x},{y}");
                // set queue retry
                message.Retry = _queueConfiguration.Value[Queues.Skip(1).FirstOrDefault()].QueueRetry + 1;
            }

            message.Retry -= 1;
            // push message to queue to retry cycle
            _ = _pushRequestLogic.Push(message);
            return Task.CompletedTask;
        }

        public async Task PushForCallBackApi(Message message, HttpResponseMessage httpResponseMessage)
        {
            // get all queue in system except CallBackFailuer
            var queues = _queueConfiguration.Value.Keys.Where(x => x != "CallBackFailuer").Aggregate((x, y) => $"{x},{y}");
            var conent = await httpResponseMessage.Content?.ReadAsStringAsync();
            var headers = httpResponseMessage.Headers?.ToDictionary(k => k.Key, k => k.Value.Aggregate((x, y) => x + y));
            headers.Add("TargetCallStatusCode", Convert.ToInt32(httpResponseMessage.StatusCode).ToString());
            // push message to rabbitmq and replace call back with target 
            _ = _pushRequestLogic.Push(new Message
            {
                TargetRequest = new Domain.Model.Request.PushRequest
                {
                    ContentBody = conent,
                    Url = message.CallBackRequest.Url,
                    ContentType = message.CallBackRequest.ContentType,
                    MethodVerb = MethodVerb.Post,
                    ServiceType = message.CallBackRequest.ServiceType,
                    SoapAction = message.CallBackRequest.SoapAction
                },
                TargetOAuthRequest = message.CallBackOAuthRequest,
                IsCallBackMessage = true,
                Queues = queues,
                ReferenceNumber = message.ReferenceNumber,
                Retry = _queueConfiguration.Value.Values.First().QueueRetry,
                HttpStatusCode = (int)httpResponseMessage.StatusCode,
                Headers = headers

            });
        }

        public Task FailureLogic(Message message, HttpResponseMessage httpResponseMessage)
        {
            var isSucess = ((int)httpResponseMessage.StatusCode >= 200) && ((int)httpResponseMessage.StatusCode <= 299);
            // check if message for traget 
            if (message.IsCallBackMessage)
            {
                // update call back Failure document  
                return _CallBackFailureRepository.UpdateFaulier(message.ReferenceNumber, isSucess);
            }

            // if success send to call back queue    
            if (isSucess)
                _ = PushForCallBackApi(message, httpResponseMessage);

            // update target Failure document 
            return _TargetFailuerRepository.UpdateFaulier(message.ReferenceNumber, isSucess);
        }
    }
}