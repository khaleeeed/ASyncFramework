using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using Newtonsoft.Json;
using Quartz;
using QuartzManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzManager.JobManager
{
    [DisallowConcurrentExecution]
    public class RecoveryFauilerJob : IJob
    {
        private readonly IQueueConfigurationService _QueueConfiguration;
        private readonly IRabbitProducers _RabbitProducers;
        private readonly IPushFailuerRepository _PushFailuerRepository;
        public RecoveryFauilerJob(IPushFailuerRepository pushFailuerRepository, IQueueConfigurationService queueConfigurationService, IRabbitProducers rabbitProducers)
        {
            _RabbitProducers = rabbitProducers;
            _QueueConfiguration = queueConfigurationService;
            _PushFailuerRepository = pushFailuerRepository;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var PushFailuerEntities = await _PushFailuerRepository.GetAll();

                foreach (var entity in PushFailuerEntities)
                {
                    var request = JsonConvert.DeserializeObject<PushRequestCommand>(entity.Notification.Request);
                    var Headrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(entity.Notification.Header);

                    QueueConfigurations queueConfiguration = _QueueConfiguration.QueueConfiguration[((IEnumerable<string>)request.Queues.Split(",", StringSplitOptions.None)).First()];

                    Message message = new Message()
                    {
                        CallBackRequest = request.CallBackRequest,
                        TargetRequest = request.TargetRequest.TargetServiceRequest,
                        Queues = entity.Queues,
                        Retry = entity.Retry,
                        IsCallBackMessage=entity.IsCallBackMessage,
                        IsFailureMessage=entity.IsFailureMessage,
                        ReferenceNumber = Convert.ToString(entity.NotificationId),
                        Headers = Headrs,
                        SystemCode = entity.Notification.SystemCode,
                        CallBackQueues = request.Queues,
                        ServiceCode=entity.Notification.ServiceCode ?? 0                         
                    };

                    _RabbitProducers.PushMessage(message, queueConfiguration);

                    await _PushFailuerRepository.UpdateIsActive(entity.Id);
                }

            }
            catch (Exception ex)
            {


            }   
        }
    }
}