using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Application.Manager.Report.Query.GetCallBackFailureCountBySystemHandler;
using ASyncFramework.Application.Manager.Report.Query.GetCallBackFailureCountHandler;
using ASyncFramework.Application.Manager.Report.Query.GetFinshNotificationCountBySystemHandler;
using ASyncFramework.Application.Manager.Report.Query.GetFinshNotificationCountHandler;
using ASyncFramework.Application.Manager.Report.Query.GetMessageInProcessCountBySystemHandler;
using ASyncFramework.Application.Manager.Report.Query.GetMessageInProcessCountHandler;
using ASyncFramework.Application.Manager.Report.Query.GetMessageInProcessDeatilsHandler;
using ASyncFramework.Application.Manager.Report.Query.GetMessageInProcessDeatilsSystemHandler;
using ASyncFramework.Application.Manager.Report.Query.GetNotificationCountBySystemHandler;
using ASyncFramework.Application.Manager.Report.Query.GetNotificationCountHandler;
using ASyncFramework.Application.Manager.Report.Query.GetNotificationDeatilsHandler;
using ASyncFramework.Application.Manager.Report.Query.GetNotificationDeatilsSystemHandler;
using ASyncFramework.Application.Manager.Report.Query.GetTargetFailureCountBySystemHandler;
using ASyncFramework.Application.Manager.Report.Query.GetTargetFailureCountHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ApiController
    {
        /// <summary>
        /// Get notification Count     
        /// </summary>
        [HttpGet("notificationCount")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get([FromQuery] GetNotificationCountQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }
        /// <summary>
        /// Get notification Details     
        /// </summary>
        [HttpGet("notificationDetails")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetDetails([FromQuery] GetNotificationDetailsQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        /// <summary>
        /// Get notification Details     
        /// </summary>
        [HttpGet("notificationDetails/system")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> GetDetails([FromQuery] GetNotificationDeatilsSystemQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        /// <summary>
        /// Get notification Count        
        /// </summary>
        [HttpGet("notificationCount/system")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] NotificationCountBySystemQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        /// <summary>
        /// Get notification Count         
        /// </summary>
        [HttpGet("finshNotificationCount")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] GetFinshNotificationCountQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }
        /// <summary>
        /// Get notification Count        
        /// </summary>
        [HttpGet("finshNotificationCount/system")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] GetFinshNotificationCountBySystemQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }


        /// <summary>
        /// Get targetFailure Count        
        /// </summary>
        [HttpGet("targetFailure")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get([FromQuery] TargetFailureCountQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }
        /// <summary>
        /// Get targetFauiler Count for system Count        
        /// </summary>
        [HttpGet("targetFailure/system")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] TargetFailureCountBySystemQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        /// <summary>
        /// Get callBackFailure Count        
        /// </summary>
        [HttpGet("callbackFailure")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get([FromQuery] CallBackFailureCountQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }
        /// <summary>
        /// Get callBackFailure for system Count        
        /// </summary>
        [HttpGet("callbackFailure/system")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] CallBackFailureCountBySystemQuery queryMessageStatus)
        {
            var response = await Mediator.Send(queryMessageStatus);
            return new JsonResult(response);
        }

        /// <summary>
        /// Get notification in process Count        
        /// </summary>
        [HttpGet("inProcess")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get([FromQuery] MessageInProcessCountQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }
        /// <summary>
        /// Get notification in proccess for system Count        
        /// </summary>
        [HttpGet("inProcess/system")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] MessageInProcessCountBySystemQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }
        /// <summary>
        /// Get notification in process Count        
        /// </summary>
        [HttpGet("inProcessDetails")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetDetails([FromQuery] GetMessageInProcessDeatilsQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        /// <summary>
        /// Get notification in process Count        
        /// </summary>
        [HttpGet("inProcessDetails/System")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> GetSystemDetails([FromQuery] GetMessageInProcessDeatilsSystemQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }


    }
}
