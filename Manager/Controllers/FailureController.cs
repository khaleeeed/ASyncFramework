using System.Collections.Generic;
using System.Threading.Tasks;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendCallBackFailuresCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendListCallBackFailureCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendListTargetFailuresCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendTargetFailureCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.CallBackFailuerMessagesQuery.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.CallBackSystemFailuerMessagesQuery.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetFailuerMessagesQuery.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetSystemFailuerMessagesQuery.Handler;
using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FailureController : ApiController
    {
        /// <summary>
        /// Get all target failuer message 
        /// </summary>
        [HttpGet("target/messages")]
        public async Task<ActionResult<GenericDocumentResponse<TargetFailuerDocument>>> GetTarget(int from)
        {
            var response = await Mediator.Send(new TargetFailuerMessagesQuery { From = from });

            if (response.Document == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;
        }

        /// <summary>
        /// Get all taget failuer message for system  
        /// </summary>
        [HttpGet("target/messages/systemcode/{systemCode}")]
        public async Task<ActionResult<GenericDocumentResponse<TargetFailuerDocument>>> GetTarget(string systemCode, int from)
        {
            var response = await Mediator.Send(new TargetSystemFailuerMessagesQuery { From = from, SystemCode = systemCode });

            if (response.Document == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;
        }

        /// <summary>
        /// Get all call back failuer
        /// </summary>     
        [HttpGet("callback/messages")]
        public async Task<ActionResult<GenericDocumentResponse<CallBackFailuerDocument>>> GetCallback(int from)
        {
            var response = await Mediator.Send(new CallBackFailuerMessagesQuery { From = from });

            if (response.Document == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;
        }

        /// <summary>
        /// Get all call back failuer
        /// </summary>                        
        [HttpGet("callback/messages/systemcode/{systemCode}")]
        public async Task<ActionResult<GenericDocumentResponse<CallBackFailuerDocument>>> GetCallback(string systemCode, int from)
        {
            var response = await Mediator.Send(new CallBackSystemFailuerMessagesQuery { From = from,SystemCode = systemCode });

            if (response.Document == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;

        }

        /// <summary>
        /// Retry send target failuer
        /// </summary>
        [HttpPost("target")]
        public async Task<ActionResult<Result>> PostTarget(string referenceNumber)
        {
            var response = await Mediator.Send(new RetrySendTargetFailureCommand { ReferenceNumber = referenceNumber });

            if (response.Succeeded == false)
            {
                return NotFound(response);
            }

            return response;
        }

        /// <summary>
        /// Retry send list of target failuer
        /// </summary>
        [HttpPost("/api/failures/target")]
        public async Task<ActionResult<Result>> PostTarget(List<string> refrenceNumbers)
        {
            var response = await Mediator.Send(new RetrySendListTargetFailuresCommand { ReferenceNumbers = refrenceNumbers });

            if (response.Succeeded == false)
            {
                return NotFound(response);
            }

            return response;
        }

        /// <summary>
        /// Retry send list of callback failuer
        /// </summary>
        [HttpPost("callback")]
        public async Task<ActionResult<Result>> PostCallback(string referenceNumber)
        {
            var response = await Mediator.Send(new RetrySendCallBackFailureCommand { ReferenceNumber=referenceNumber});

            if (response.Succeeded == false)
            {                
                return NotFound(response);
            }

            return response;
        }

        /// <summary>
        /// Retry send list of callback failuer
        /// </summary>
        [HttpPost("/api/failures/callback")]
        public async Task<ActionResult<Result>> PostCallback(List<string> refrenceNumbers)
        {
            var response = await Mediator.Send(new RetrySendListCallBackFailuresCommand { ReferenceNumbers=refrenceNumbers });

            if (response.Succeeded == false)
            {
                return NotFound(response);
            }

            return response;
        }      
    }
}