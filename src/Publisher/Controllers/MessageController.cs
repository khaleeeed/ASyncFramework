using System.Threading.Tasks;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.PushRequestLogic;
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
    }
}
