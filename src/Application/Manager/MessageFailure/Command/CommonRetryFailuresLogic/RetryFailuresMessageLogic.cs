using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Logic;
using ASyncFramework.Domain.Model;
using ASyncFramework.Domain.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic
{
    public class RetryFailuresMessageLogic : IRetryFailuresMessageLogic
    {
        private readonly IPushRequestLogic _PushRequestLogic;

        public RetryFailuresMessageLogic(IPushRequestLogic pushRequestLogic)
        {
            _PushRequestLogic = pushRequestLogic;
        }

        public async Task<Result> Retry(string messageJson)
        {
            // Deserialize message 
            var message = System.Text.Json.JsonSerializer.Deserialize<Message>(messageJson);

            // set failure property
            message.IsFailureMessage = true;
            message.Retry = 0;
            message.Queues = "1";

            // push message to queue
            _ =await _PushRequestLogic.Push(message);
            return new Result(true, null) { ReferenceNumber = message.ReferenceNumber };
        }
    }
}
