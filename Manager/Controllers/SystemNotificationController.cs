using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Manager.SystemNotification.Command.CreateSystemCommand.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemNotificationController : ApiController
    {
        /// <summary>
        /// Create new system
        /// </summary>        
        [HttpPost]
        public Task<Result> Post(CreateSystemCommand command)
        {
            return  Mediator.Send(command);
        }

        /// <summary>
        /// Update System
        /// </summary>        
        [HttpPut]
        public Task<Result> Put()
        {
            throw new NotImplementedException();
        }
    }
}