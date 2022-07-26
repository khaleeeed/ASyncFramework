using System.Threading.Tasks;
using ASyncFramework.Application.Manager.Subscriber.Command.ForceStopAllSubscriber;
using ASyncFramework.Application.Manager.Subscriber.Command.ForceStopSubscriber;
using ASyncFramework.Application.Manager.Subscriber.Command.StartAllSubscriber;
using ASyncFramework.Application.Manager.Subscriber.Command.StartSubscriber;
using ASyncFramework.Application.Manager.Subscriber.Command.StopAllSubscriber;
using ASyncFramework.Application.Manager.Subscriber.Command.StopSubscriber;
using ASyncFramework.Application.Manager.Subscriber.Query.GetAllSubscriber;
using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class SubscriberController : ApiController
    {

        /// <summary>
        /// Get all Subscriber   
        /// </summary>
        [HttpGet]
        public async Task<GenericDocumentResponse<SubscriberEntity>> GetAllSubscriber()
        {
            var response = await Mediator.Send(new GetAllSubscriberQuery());
            return response;
        }

        /// <summary>
        /// stop subscriber 
        /// </summary>
        [HttpPost("stop")]
        public async Task<string> Stop(StopSubscriberCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        /// stop all sbuscriber 
        /// </summary>
        [HttpPost("/api/subscribers/stop")]
        public async Task<Result> Stop(StopAllSubscriberCommand command)
        {
            var response = await Mediator.Send(command);
            return response;

        }

        /// <summary>
        /// force stop subscriber 
        /// </summary>
        [HttpPost("forceStop")]
        public async Task<string> ForceStop(ForceStopSubscriberCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        /// force stop all sbuscriber 
        /// </summary>
        [HttpPost("/api/subscribers/forceStop")]
        public async Task<Result> ForceStop(ForceStopAllSubscriberCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        /// start subscriber 
        /// </summary>
        [HttpPost("start")]
        public async Task<string> Start(StartSubscriberCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

        /// <summary>
        ///start all subscriber
        /// </summary>
        [HttpPost("/api/subscribers/start")]
        public async Task<Result> Start(StartAllSubscriberCommand command)
        {
            var response = await Mediator.Send(command);
            return response;
        }

    }
}
