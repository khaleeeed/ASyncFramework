using System.Threading.Tasks;
using ASyncFramework.Application.Manager.Systems.Command.AddSystem;
using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Mvc;
using ASyncFramework.Application.Manager.Systems.Query.GetAllSystemQuery;
using ASyncFramework.Application.Manager.Systems.Query.GetSystemQuery;
using ASyncFramework.Application.Manager.Systems.Command.UpdateSystem;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ApiController
    {
        /// <summary>
        /// Get all Systems   
        /// </summary>  
        [HttpGet("/api/GetAllSystem")]
        public async Task<GenericDocumentResponse<SystemEntity>> GetAllQueueConfigurations()
        {
            var response = await Mediator.Send(new GetAllSystemQuery());
            return response;
        }

        /// <summary>
        /// add new queue 
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Result>> AddNewQueue(AddSystemCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        /// Get system
        /// </summary>
        [HttpGet]
        public Task<SystemEntity> GetSystem(string systemCode)
        {
            return Mediator.Send(new GetSystemQuery() { SystemCode = systemCode });
        }

        /// <summary>
        /// update system
        /// </summary>        
        [HttpPost("/api/updateSystem")]
        public async Task<ActionResult<Result>> UpdateQueue(UpdateSystemCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }
    }
}
