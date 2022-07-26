using System.Collections.Generic;
using System.Threading.Tasks;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Application.QueryQueueDescription;
using ASyncFramework.Domain.Model;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ApiController
    {
        /// <summary>
        /// Generate new request in async 
        /// </summary>
        [HttpPost]        
        public async Task<ActionResult<Result>> Post([FromBody] PushRequestCommand command)
        {
            if (command==null)
                return UnprocessableEntity(new Result(false, new List<string> { "PushRequestCommand required" }));

            // meditator call PushRequestCommandHandler.Handle method in application lib
            var result = await Mediator.Send(command);

            if (!result.Succeeded)
                return UnprocessableEntity(result);

            return result;
        }

        /// <summary>
        /// Get Description for available queue
        /// </summary>
        [HttpGet("queue")]
        public async Task<List<QueueDescription>> Get()
        {
            // meditator call GetDescriptionQueryHandler.Handle method in application lib             
            return await Mediator.Send(new GetDescriptionQuery());
        }

        /// <summary>
        /// Generate new request in async 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Result>> Get([FromQuery] PushRequestCommand command)
        {
            return await Post(command);
        }

        /// <summary>
        /// Dispose Request
        /// </summary>
        [HttpDelete("Dispose")]
        public async Task<ActionResult<Result>> post()
        {
            throw new System.Exception();
        }
    }
}