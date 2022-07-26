using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Application.Manager.MessageQuery.MessageStatus.Handler;
using ASyncFramework.Application.Manager.MessageQuery.ResponseForCallBackApi.Handler;
using ASyncFramework.Application.Manager.MessageQuery.ResponseForTargetApi.Handler;
using ASyncFramework.Application.Manager.MessageQuery.SearchByCallBackUrl.Handler;
using ASyncFramework.Application.Manager.MessageQuery.SearchByContentBodyForAdmin.Handler;
using ASyncFramework.Application.Manager.MessageQuery.SearchByContentBodyForSystem.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ApiController
    {
         /// <summary>
         /// Get status of message         
         /// </summary>
        [HttpGet("status")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] MessageStatusQuery queryMessageStatus)
        {
           var response= await  Mediator.Send(queryMessageStatus);
           return new JsonResult(response);
        }

        /// <summary>
        /// Retrive target api response
        /// </summary>
        [HttpGet("targetResponse")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] TargetResponseQuery targetResponseQuery)
        {
            var response = await Mediator.Send(targetResponseQuery);
            return new JsonResult(response);
        }

        /// <summary>
        /// Retrieve callback response
        /// </summary>
        [HttpGet("callBackResponse")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] CallBackResponseQuery callBackResponseQuery)
        {
            var response = await Mediator.Send(callBackResponseQuery);
            return new JsonResult(response);
        }

        /// <summary>
        /// Search message with callBack url 
        /// </summary>
        [HttpGet("callBackUrl")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] CallBackUrlQuery callBackUrlQuery)
        {
            var response = await Mediator.Send(callBackUrlQuery);
            return new JsonResult(response);
        }

        /// <summary>
        /// Search with content body for admin
        /// </summary>
        [HttpGet("ContentBodyForAdmin")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get([FromQuery] SearchByContentBodyForAdminQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        /// <summary>
        /// Search with content body for system
        /// </summary>
        [HttpGet("ContentBodyForSystem")]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get([FromQuery] SearchByContentBodyForSystemQuery query)
        {
            var response = await Mediator.Send(query);
            return new JsonResult(response);
        }
    }
}