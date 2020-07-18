using System.Collections.Generic;
using System.Threading.Tasks;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Application.QueryQueueDescription;
using ASyncFramework.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Publisher.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ApiController
    {
        [HttpPost]
        public async Task<Result> Post([FromBody]PushRequestCommand command)
        {
             return await Mediator.Send(command);
        }

        [HttpGet("queue")]
        public async Task<List<QueueDescription>> Get ()
        {
            return await Mediator.Send(new GetDescriptionQuery());
        }
    }
}
