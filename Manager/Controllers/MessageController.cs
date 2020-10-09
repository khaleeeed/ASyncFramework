using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Application.Manager.MessageQuery.MessageStatus.Handler;
using ASyncFramework.Application.Manager.MessageQuery.ResponseForCallBackApi.Handler;
using ASyncFramework.Application.Manager.MessageQuery.ResponseForTargetApi.Handler;
using ASyncFramework.Application.Manager.MessageQuery.SearchByCallBackUrl.Handler;
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
        public async Task<IActionResult> Get([FromQuery] MessageStatusQuery queryMessageStatus)
        {
           var response= await  Mediator.Send(queryMessageStatus);
            return new JsonResult(response, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        /// <summary>
        /// Retrive target api response
        /// </summary>
        [HttpGet("targetResponse")]
        public async Task<IActionResult> Get([FromQuery] TargetResponseQuery targetResponseQuery)
        {
            var response = await Mediator.Send(targetResponseQuery);
            return new JsonResult(response, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        /// <summary>
        /// Retrive callback response
        /// </summary>
        [HttpGet("callBackResponse")]
        public async Task<IActionResult> Get([FromQuery] CallBackResponseQuery callBackResponseQuery)
        {
            var response = await Mediator.Send(callBackResponseQuery);
            return new JsonResult(response, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        /// <summary>
        /// Search message with callBack url 
        /// </summary>
        [HttpGet("callBackUrl")]
        public async Task<IActionResult> Get([FromQuery] CallBackUrlQuery callBackUrlQuery)
        {
            var response = await Mediator.Send(callBackUrlQuery);
            return new JsonResult(response, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

    }
}