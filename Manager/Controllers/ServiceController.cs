using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Application.Manager.Service.Command.AddService;
using ASyncFramework.Application.Manager.Service.Query.GetAllService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ApiController
    {
        /// <summary>
        /// Get all service
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Get(string systemCode)
        {
            var response = await Mediator.Send(new GetAllServiceQuery { SystemCode=systemCode});
            return new JsonResult(response);
        }

        /// <summary>
        /// Add new service
        /// </summary>
        [HttpPost()]
        [Authorize(Roles = "ADMIN,SYSTEM")]
        public async Task<IActionResult> Post(AddServiceCommand command)
        {
            var response = await Mediator.Send(command);
            return new JsonResult(response);
        }
    }
}
