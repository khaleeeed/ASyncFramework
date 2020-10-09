using System;
using System.Threading.Tasks;
using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Manager.ManagerNotification.Command.CreateNewNotificationCommand.Handler;
using ASyncFramework.Application.Manager.ManagerNotification.Query.GetAllNotification.Handler;
using ASyncFramework.Application.Manager.ManagerNotification.Query.GetAllNotificationBySystemCode.Handler;
using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ApiController
    {
        /// <summary>
        /// create new notification 
        /// </summary>
        [HttpPost]
        public Task<Result> Post(CreateNotificationCommand command)
        {
            return Mediator.Send(command);
        }

        /// <summary>
        /// update notification
        /// </summary>        
        [HttpPut]
        public Task<Result> Put()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get all notification 
        /// </summary>
        [HttpGet]
        public Task<GenericDocumentResponse<NotificationDocument>> Get(int from)
        {
            return Mediator.Send(new GetAllNotificationQuery { From = from });
        }

        /// <summary>
        /// get all notification by SystemCode 
        /// </summary>
        [HttpGet("system")]
        public Task<GenericDocumentResponse<NotificationDocument>>Get(string systemCode, int from)
        {
            return Mediator.Send(new GetAllNotificationBySystemCode { SystemCode = systemCode , From = from });
        }
    }
}