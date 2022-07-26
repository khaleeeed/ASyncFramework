using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.QueueConfiguration.Command.AddQueue
{
    public class AddQueueHandler : IRequestHandler<AddQueueCommand, Result>
    {
        private readonly IQueueConfigurationRepository _QueueConfigurationRepository;

        public AddQueueHandler(IQueueConfigurationRepository queueConfigurationRepository)
        {
            _QueueConfigurationRepository = queueConfigurationRepository;
        }

        public async Task<Result> Handle(AddQueueCommand request, CancellationToken cancellationToken)
        {            
            var isAdd = await _QueueConfigurationRepository.Add(request.data);
            if (!isAdd)
            {
                return new Result(false, new List<string> { "Queue not add" });
            }

            return new Result(true, null);
        }
    }

    public class AddQueueCommand:IRequest<Result>
    {
        public QueueConfigurations data { get; set; }
    }
}
