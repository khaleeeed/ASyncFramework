using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem.QueueSubscriber;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriber.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        private readonly RunTimeQueue _RunTimeQueue;
        private readonly IQueueConfigurationService _QueueConfigurationService;
        private readonly ISubscriberRepository _SubscriberRepository;

        public SubscriberController(RunTimeQueue runTimeQueue, IQueueConfigurationService queueConfigurationService, ISubscriberRepository subscriberRepository)
        {
            _RunTimeQueue = runTimeQueue;
            _QueueConfigurationService = queueConfigurationService;
            _SubscriberRepository = subscriberRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Result>> Stop(byte[] timeStampCheck)
        {
            var isUpdated = await _SubscriberRepository.UpdateIsRunning(false, timeStampCheck);
            if (!isUpdated || !_RunTimeQueue.IsRunning)
                return BadRequest(new Result(false, new List<string> { "Data updated please refresh page" }));

            if (_RunTimeQueue.IsRunning)
                _ = _RunTimeQueue.StopAsync(CancellationToken.None);

            return new Result(true, null);
        }

        [HttpPost]
        public async Task<ActionResult<Result>> ForceStop(byte[] timeStampCheck)
        {
            var isUpdated = await _SubscriberRepository.UpdateIsRunning(false, timeStampCheck);
            if (!isUpdated)
                return BadRequest(new Result(false, new List<string> { "Data updated please refresh page" }));

            _=_RunTimeQueue.StopAsync(CancellationToken.None);

            return new Result(true, null);
        }

        [HttpPost]
        public async Task<ActionResult<Result>> Start(byte[] timeStampCheck)
        {
            var isUpdated = await _SubscriberRepository.UpdateIsRunning(true, timeStampCheck);
            if (!isUpdated  || _RunTimeQueue.IsRunning)
                return BadRequest(new Result(false, new List<string> { "Data updated please refresh page" }));

            if (!_RunTimeQueue.IsRunning)
                _=_RunTimeQueue.StartAsync(CancellationToken.None);

            return new Result(true, null);
        }

        [HttpPost]
        public async Task<ActionResult> RefreshQueueConfiguration()
        {
            _QueueConfigurationService.UpdateQueueConfiguration();
            _RunTimeQueue.ReCreateInstance();
            await _SubscriberRepository.UpdateTimeOfTakeConfiguration(DateTime.Now);
            return Ok();
        }
    }
}