using System.Threading.Tasks;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendCallBackFailuresCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendListCallBackFailureCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendListTargetFailuresCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendTargetFailureCommand.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.CallBackFailuerMessagesQuery.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.CallBackSystemFailuerMessagesQuery.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetFailuerMessagesQuery.Handler;
using ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetSystemFailuerMessagesQuery.Handler;
using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FailureController : ApiController
    {
        /// <summary>
        /// Get all target failure message 
        /// </summary>
        [HttpGet("target/messages")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<GenericDocumentResponse<TargetFailuerEntity>>> GetTarget(int from)
        {
            var response = await Mediator.Send(new TargetFailuerMessagesQuery { From = from });

            if (response.data == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;
        }

        /// <summary>
        /// Get all target failure message for system   
        /// </summary>
        [HttpGet("target/messages/systemcode/{systemCode}")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<ActionResult<GenericDocumentResponse<TargetFailuerEntity>>> GetTarget(string systemCode, int from)
        {
            var response = await Mediator.Send(new TargetSystemFailuerMessagesQuery { From = from, SystemCode = systemCode });

            if (response.data == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;
        }

        /// <summary>
        /// Get all call back failure
        /// </summary>     
        [HttpGet("callback/messages")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<GenericDocumentResponse<CallBackFailuerEntity>>> GetCallback(int from)
        {
            var response = await Mediator.Send(new CallBackFailuerMessagesQuery { From = from });

            if (response.data == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;
        }

        /// <summary>
        /// Get all call back failure
        /// </summary>                        
        [HttpGet("callback/messages/systemcode/{systemCode}")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<ActionResult<GenericDocumentResponse<CallBackFailuerEntity>>> GetCallback(string systemCode, int from)
        {
            var response = await Mediator.Send(new CallBackSystemFailuerMessagesQuery { From = from, SystemCode = systemCode });

            if (response.data == null)
            {
                response.Message = "No Document found";
                return NotFound(response);
            }
            response.Message = "OK";
            return response;

        }

        /// <summary>
        /// Retry send target failure
        /// </summary>
        [HttpPost("target")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
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
        /// Retry send list of target failure
        /// </summary>
        [HttpPost("/api/failures/target")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<ActionResult<Result>> PostTarget(RetrySendListTargetFailuresCommand refrenceNumbers)
        {
            var response = await Mediator.Send(refrenceNumbers);

            if (response.Succeeded == false)
            {
                return NotFound(response);
            }

            return response;
        }

        /// <summary>
        /// Retry send list of callback failure
        /// </summary>
        [HttpPost("callback")]
        [Authorize(Roles = "ADMIN")]
        [Authorize(Roles = "SYSTEM")]
        public async Task<ActionResult<Result>> PostCallback(string referenceNumber)
        {
            var response = await Mediator.Send(new RetrySendCallBackFailureCommand { ReferenceNumber = referenceNumber });

            if (response.Succeeded == false)
            {
                return NotFound(response);
            }

            return response;
        }

        /// <summary>
        /// Retry send list of callback failure
        /// </summary>
        [HttpPost("/api/failures/callback")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<ActionResult<Result>> PostCallback(RetrySendListTargetFailuresCommand command)
        {
            var response = await Mediator.Send(new RetrySendListCallBackFailuresCommand { ReferenceNumbers = command.ReferenceNumbers, TimeStampChecks = command.TimeStampChecks });

            if (response.Succeeded == false)
            {
                return NotFound(response);
            }

            return response;
        }
    }
}