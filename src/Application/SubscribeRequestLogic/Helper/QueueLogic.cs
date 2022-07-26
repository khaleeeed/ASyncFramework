using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.Logic;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ASyncFramework.Application.SubscribeRequestLogic.Helper
{
    public class QueueLogic: IQueueLogic
    {
        private readonly IInfrastructureLogger<QueueLogic> _logger;
        private readonly ICallBackFailureRepository _CallBackFailureRepository;
        private readonly ITargetFailuerRepository _TargetFailuerRepository;
        private readonly IQueueConfigurationService _queueConfiguration;
        private readonly IPushRequestLogic _pushRequestLogic;
        private readonly INotificationRepository _notificationRepository;

        public QueueLogic(IInfrastructureLogger<QueueLogic> logger, ICallBackFailureRepository callBackFailureRepository, ITargetFailuerRepository targetFailuerRepository, IQueueConfigurationService queueConfiguration, IPushRequestLogic pushRequestLogic, INotificationRepository notificationRepository)
        {
            _logger = logger;
            _CallBackFailureRepository = callBackFailureRepository;
            _TargetFailuerRepository = targetFailuerRepository;
            _queueConfiguration = queueConfiguration;
            _pushRequestLogic = pushRequestLogic;
            _notificationRepository = notificationRepository;
        }

        public Task Retry(Message message, int statusCode, bool isMessageCallBack = false)
        {
            // if call return client error finsh request 
            if (isMessageCallBack && (statusCode>=400 && statusCode < 500))
            {
                // get CallBackFailuer configuration and push message to call back fauiler 
                _logger.LogFinish(DateTime.Now, MessageLifeCycle.CallBackUsedAllRetries, message.ReferenceNumber);

                _notificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.CallBackUsedAllRetries);

                // add call back fauiler to elk index 
                _ = _CallBackFailureRepository.Add(new Domain.Entities.CallBackFailuerEntity
                {
                    CallBackUrl = message.TargetRequest.Url,
                    IsSendSuccessfully = false,
                    NotificationId = Convert.ToInt64(message.ReferenceNumber),
                    ContentBody = message.TargetRequest.ContentBody,
                    StatusCode = statusCode,
                    CreationDate = DateTime.Now,
                    Retry = 0,
                    SystemCode = message.SystemCode,
                    IsProcessing = false,
                    Message = System.Text.Json.JsonSerializer.Serialize(message)

                });
                return Task.CompletedTask;
            }

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
                         _logger.LogFinish(DateTime.Now,MessageLifeCycle.CallBackUsedAllRetries, message.ReferenceNumber);

                        _notificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.CallBackUsedAllRetries);

                        // add call back fauiler to elk index 
                        _ = _CallBackFailureRepository.Add(new Domain.Entities.CallBackFailuerEntity
                        {
                            CallBackUrl = message.TargetRequest.Url,
                            IsSendSuccessfully = false,
                            NotificationId = Convert.ToInt64(message.ReferenceNumber),
                            ContentBody = message.TargetRequest.ContentBody,
                            StatusCode = statusCode,
                            CreationDate = DateTime.Now,
                            Retry = 0,
                            SystemCode = message.SystemCode,                            
                            IsProcessing = false,
                            Message = System.Text.Json.JsonSerializer.Serialize(message)
                        });
                        return Task.CompletedTask;
                    }

                    // if there no next queue available and message in target request. message will lost 
                    _logger.LogFinish(DateTime.Now, MessageLifeCycle.TargetUsedAllRetries, message.ReferenceNumber);

                    _notificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.CallBackUsedAllRetries);
                    
                    string callbackUrl = string.Empty;
                    message.CallBackRequest?.ForEach(x => callbackUrl = callbackUrl + " " + x.CallBackServiceRequest.Url);
                    // add taget faiuler to elk index 
                    _ = _TargetFailuerRepository.Add(new Domain.Entities.TargetFailuerEntity
                    {
                        CallBackUrl = callbackUrl,
                        ContentBody = message.TargetRequest.ContentBody,
                        IsSendSuccessfully = false,
                        Method = message.TargetRequest.MethodVerb.ToString(),
                        NotificationId = message.ReferenceNumber,
                        CreationDate = DateTime.Now,
                        Retry = 0,
                        StatusCode = statusCode,
                        SystemCode = message.SystemCode,                     
                        IsProcessing = false,
                        Message = System.Text.Json.JsonSerializer.Serialize(message)
                    });
                    return Task.CompletedTask ;
                }

                // set all another queue and skip queue UsedAllRetries
                message.Queues = Queues.Skip(1).Aggregate((x, y) => $"{x},{y}");
                // set queue retry
                message.Retry = _queueConfiguration.QueueConfiguration[Queues.Skip(1).FirstOrDefault()].QueueRetry + 1;
            }

            message.Retry -= 1;
            // logger 

            if (!isMessageCallBack)
                _logger.LogRetry(DateTime.Now,MessageLifeCycle.RetrySendingTarget, message.ReferenceNumber, message.Queues.Split(',').First(), message.Retry);
            else
                _logger.LogRetry(DateTime.Now, MessageLifeCycle.RetrySendingCallBack,message.ReferenceNumber, message.Queues.Split(',').First(), message.Retry);

            // push message to queue to retry cycle
            _ = _pushRequestLogic.Push(message);
            return Task.CompletedTask;
        }

        public async Task PushForCallBackApi(Message message, HttpResponseMessage httpResponseMessage)
        {
            // get all queue in system except CallBackFailuer
            var queues = message.CallBackQueues;  // callback for all queue in system /*_queueConfiguration.Value.Keys.Where(x => x != "CallBackFailuer").Aggregate((x, y) => $"{x},{y}")*/;
            var conent = await httpResponseMessage.Content?.ReadAsStringAsync();
            var headers = httpResponseMessage.Headers?.ToDictionary(k => k.Key, k => k.Value.Aggregate((x, y) => x + y));            
            headers.Add("ASyncTargetCallStatusCode", Convert.ToInt32(httpResponseMessage.StatusCode).ToString());
            headers.Add("ASyncExtraInfo", message.ExtraInfo);
            headers.Add("ASyncReferenceNumber", message.ReferenceNumber);
            // push message to rabbitmq and replace call back with target 
            foreach (var callBackRequest in message.CallBackRequest)
            {
                if (callBackRequest.CallBackServiceRequest.Headers != null && callBackRequest.CallBackServiceRequest.Headers.Count > 0)
                    headers = headers.Concat(callBackRequest.CallBackServiceRequest.Headers).ToDictionary(x => x.Key, x => x.Value);

                _ = _pushRequestLogic.Push(new Message
                {
                    TargetRequest = new Domain.Model.Request.PushRequest
                    {
                        ContentBody = conent,
                        Url = callBackRequest.CallBackServiceRequest.Url,
                        ContentType = callBackRequest.CallBackServiceRequest.ContentType,
                        MethodVerb = MethodVerb.Post,
                        ServiceType = callBackRequest.CallBackServiceRequest.ServiceType,
                        SoapAction = callBackRequest.CallBackServiceRequest.SoapAction                      
                    },
                    IsCallBackMessage = true,
                    Queues = queues,
                    ReferenceNumber = message.ReferenceNumber,
                    Retry = _queueConfiguration.QueueConfiguration.Values.First().QueueRetry,
                    HttpStatusCode = (int)httpResponseMessage.StatusCode,
                    Headers = headers,
                    SystemCode=message.SystemCode,
                    ServiceCode=message.ServiceCode

                });
            }
            message.CallBackRequest = null;
           
        }

        public Task FailureLogic(Message message, HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage==null)
            {
                return _TargetFailuerRepository.UpdateFaulier(message.ReferenceNumber, false);
            }

            var isSucess = ((int)httpResponseMessage.StatusCode >= 200) && ((int)httpResponseMessage.StatusCode <= 299);
            // check if message for traget 
            if (message.IsCallBackMessage)
            {
                _logger.LogFinish(DateTime.Now, MessageLifeCycle.Succeeded, message.ReferenceNumber);

                _notificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.Succeeded);

                // update call back Failure document  
                return _CallBackFailureRepository.UpdateFaulier(message.ReferenceNumber, isSucess);
            }

            // if success send to call back queue    
            if (isSucess)
            {
                if (message.CallBackRequest != null && message.CallBackRequest.Count > 0)
                    _ = PushForCallBackApi(message, httpResponseMessage);
                else
                {
                    _logger.LogFinish(DateTime.Now, MessageLifeCycle.SucceededWithoutCallBack, message.ReferenceNumber);
                    _notificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.SucceededWithoutCallBack);
                }
            }

            // update target Failure document 
            return _TargetFailuerRepository.UpdateFaulier(message.ReferenceNumber, isSucess);
        }       
    }
}