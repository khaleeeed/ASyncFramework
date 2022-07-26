using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.QueueConfiguration.Command.UpdateQueue
{
    public class UpdateQueueHandler : IRequestHandler<UpdateQueueCommand, Result>
    {
        private readonly IQueueConfigurationRepository _QueueConfigurationRepository;

        public UpdateQueueHandler(IQueueConfigurationRepository queueConfigurationRepository)
        {
            _QueueConfigurationRepository = queueConfigurationRepository;
        }
        public async Task<Result> Handle(UpdateQueueCommand request, CancellationToken cancellationToken)
        {
            var isUpdated=await _QueueConfigurationRepository.Update(request.Id, request.QueueRetry, request.IsAutoMapping, request.NumberOfInstance, request.TimeStampCheck);

            if(!isUpdated)
                return new Result(false, new List<string> { "Data updated from last time get data please refresh page" });

            return new Result(true, null);
        }
    }

    public class UpdateQueueCommand:IRequest<Result>
    {
        public int Id { get; set; }
        public int QueueRetry { get; set; } 
        public bool IsAutoMapping { get; set; }
        public int NumberOfInstance { get; set; }
        public byte[] TimeStampCheck { get; set; }
    }
}
