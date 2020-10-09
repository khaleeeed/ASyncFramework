using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Logic;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
        private readonly IConvertObjectRequestToHttpRequestMessage _convertFromRequestToHttpRequestMessage;
        private readonly IElkLogger<SubscriberLogic> _logger;
        private readonly ISendHttpRequest _SendHttpRequest;
        private readonly IQueueLogic _QueueLogic;

        public SubscriberLogic(IConvertObjectRequestToHttpRequestMessage convertFromRequestToHttpRequestMessage, IElkLogger<SubscriberLogic> logger, ISendHttpRequest sendHttpRequest, IQueueLogic queueLogic)
        {
            _convertFromRequestToHttpRequestMessage = convertFromRequestToHttpRequestMessage;
            _logger = logger;
            _SendHttpRequest = sendHttpRequest;
            _QueueLogic = queueLogic;
        }

        public async Task Subscribe(Message message)
        {
            // try to get token if there oauth object 
            Task<Task> taskGetToken = TryGetToken(message);

            // send request 
            var httpResponseMessage = await _SendHttpRequest.SendRequest(message, taskGetToken);

            // check if fauiler message 
            if (message.IsFailureMessage)
            {
                _=_QueueLogic.FailureLogic(message, httpResponseMessage);
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
                CallBackLogic(message, httpResponseMessage);
                return;
            }

            // success or user error push message to queue for call  callbackService 
            if (httpResponseMessage.IsSuccessStatusCode || httpResponseMessage.StatusCode < System.Net.HttpStatusCode.InternalServerError)
            {
                await _QueueLogic.PushForCallBackApi(message, httpResponseMessage);
                return;
            }

            // service error retry 
            if (httpResponseMessage.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
            {
                _ = _QueueLogic.Retry(message, (int)httpResponseMessage.StatusCode);
                _logger.LogRetry(MessageLifeCycle.RetrySendingTarget, message.ReferenceNumber, message.Queues.Split(',').First(), message.Retry);
            }
        }       

        private Task<Task> TryGetToken(Message message)
        {
            Task<Task> taskGetToken = null;

            //check if the request have token 
            if (message.TargetOAuthRequest != null)
            {
                // convert  request object to httpRequestMessage 
                var taskHttpRequestMessage = Task.Run(() => _convertFromRequestToHttpRequestMessage.Convert(message.TargetOAuthRequest));

                // GetToken method will call when taskHttpRequestMessage finsh 
                taskGetToken = taskHttpRequestMessage.ContinueWith(_SendHttpRequest.GetToken);
            }

            return taskGetToken;
        }

        private void PushRetryToQueue(Message message)
        {
            // retry if cannot connect to servie 
            _ = _QueueLogic.Retry(message, 0, message.IsCallBackMessage);

            _logger.LogRetry(message.IsCallBackMessage ? MessageLifeCycle.RetrySendingCallBack : MessageLifeCycle.RetrySendingTarget,
                message.ReferenceNumber, message.Queues.Split(',').First(), message.Retry);

            return;
        }

        private void CallBackLogic(Message message, HttpResponseMessage httpResponseMessage)
        {
            // if call back message success stop function 
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogFinish(MessageLifeCycle.Succeeded, message.ReferenceNumber);
                return;
            }
            // if call back message not success retry 
            // retry and return
            _ = _QueueLogic.Retry(message, (int)httpResponseMessage.StatusCode, true);

            _logger.LogRetry(MessageLifeCycle.RetrySendingCallBack, message.ReferenceNumber, message.Queues.Split(',').First(), message.Retry);

            return;
        }
    }
}