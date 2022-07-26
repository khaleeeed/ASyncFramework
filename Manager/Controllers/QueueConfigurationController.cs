using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Application.Manager.QueueConfiguration.Command.AddQueue;
using ASyncFramework.Application.Manager.QueueConfiguration.Command.RefreshQueueConfiguration;
using ASyncFramework.Application.Manager.QueueConfiguration.Command.RefreshQueueConfiguratoinForAllSubscriber;
using ASyncFramework.Application.Manager.QueueConfiguration.Command.UpdateQueue;
using ASyncFramework.Application.Manager.QueueConfiguration.Query.GetAllQueueConfigurations;
using ASyncFramework.Application.Manager.QueueConfiguration.Query.GetQueueConfiguration;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class QueueConfigurationController : ApiController
    {
        /// <summary>
        /// Get Queue Configuration by Id
        /// </summary>
        [HttpGet]
        public Task<QueueConfigurations> GetQueueConfigurations(int id)
        {
            return Mediator.Send(new GetQueueConfigurationQuery() { ID = id });
        }

        /// <summary>
        /// Get all Queue Configurations   
        /// </summary>  
        [HttpGet("/api/QueuesConfigurations")]
        public async Task<GenericDocumentResponse<QueueConfigurations>> GetAllQueueConfigurations()
        {
            var response = await Mediator.Send(new GetAllQueueConfigurationsQuery());
            return response;
        }

        /// <summary>
        /// add new queue 
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Result>> AddNewQueue(AddQueueCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        /// update queue
        /// </summary>        
        [HttpPost("/api/updateQueuesConfigurations")]
        public async Task<ActionResult<Result>> UpdateQueue(UpdateQueueCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        /// refresh configuration 
        /// </summary>
        [HttpPost("refresh")]
        public async Task<ActionResult<Result>> RefreshConfiguration(RefreshQueueConfigurationCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        ///  refresh all subscriber confiugration 
        /// </summary>
        [HttpPost("refreshAllSubscriber")]
        public async Task<ActionResult<Result>> RefreshConfiguration()
        {
            var response = await Mediator.Send(new RefreshQueueConfiguratoinForAllSubscriberCommand());
            return response;
        }

    }
}