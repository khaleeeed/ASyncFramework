using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ASyncFramework.Application.SubscribeRequestLogic
{
    public class SubscriberLogic : ISubscriberLogic
    {
        private readonly IInfrastructureLogger<SubscriberLogic> _logger;
        private readonly ISendHttpRequest _SendHttpRequest;
        private readonly IQueueLogic _QueueLogic;
        private readonly INotificationRepository _NotificationRepository;
        private readonly IASyncAgent _Agent;
        public SubscriberLogic(IASyncAgent agent, IInfrastructureLogger<SubscriberLogic> logger, ISendHttpRequest sendHttpRequest, IQueueLogic queueLogic, INotificationRepository notificationRepository)
        {
            _logger = logger;
            _SendHttpRequest = sendHttpRequest;
            _QueueLogic = queueLogic;
            _NotificationRepository = notificationRepository;
            _Agent = agent;
        }

        public async Task Subscribe(Message message)
        {           
            // send request 
            var httpResponseMessage = await _SendHttpRequest.SendRequest(message);

            // set apm status 
            _=_Agent.SetResultToCurrentTransaction(httpResponseMessage?.IsSuccessStatusCode, httpResponseMessage?.StatusCode);

            // check if fauiler message 
            // check if message come from UI 
            if (message.IsFailureMessage)
            {
                _ = _QueueLogic.FailureLogic(message, httpResponseMessage);
                return;
            }

            // check if null cannot conncet to service 
            if (httpResponseMessage == null)
            {
                PushRetryToQueue(message);
                return;
            }

            // if call Back message 
            if (message.IsCallBackMessage)
            {
                await CallBackLogic(message, httpResponseMessage);
                return;
            }

            // success or user error push message to queue for call  callbackService 
            if (httpResponseMessage.IsSuccessStatusCode || httpResponseMessage.StatusCode < System.Net.HttpStatusCode.InternalServerError)
            {
                if (message.CallBackRequest == null)
                {
                    _logger.LogFinish(DateTime.Now, MessageLifeCycle.SucceededWithoutCallBack, message.ReferenceNumber);
                    _NotificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.SucceededWithoutCallBack);
                    return;
                }

                await _QueueLogic.PushForCallBackApi(message, httpResponseMessage);
                return;
            }

            // service error retry 
            if (httpResponseMessage.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
            {
                _ = _QueueLogic.Retry(message, (int)httpResponseMessage.StatusCode);
            }
        }

        private void PushRetryToQueue(Message message)
        {
            // retry if cannot connect to servie 
            _ = _QueueLogic.Retry(message, 0, message.IsCallBackMessage);

            return;
        }

        private Task CallBackLogic(Message message, HttpResponseMessage httpResponseMessage)
        {
            // if call back message success stop function 
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogFinish(DateTime.Now, MessageLifeCycle.Succeeded, message.ReferenceNumber);

                _NotificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.Succeeded);

                return Task.CompletedTask;
            }
            // if call back message not success retry 
            // retry and return
            _ = _QueueLogic.Retry(message, (int)httpResponseMessage.StatusCode, true);

            return Task.CompletedTask;
        }

        public async Task InternalExceptionRetry(Message message)
        {
            await _QueueLogic.Retry(message, 500, message.IsCallBackMessage);
        }
    }
}