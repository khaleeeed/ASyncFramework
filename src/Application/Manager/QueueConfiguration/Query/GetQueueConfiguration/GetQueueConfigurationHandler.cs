using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.QueueConfiguration.Query.GetQueueConfiguration
{
    public class GetQueueConfigurationHandler : IRequestHandler<GetQueueConfigurationQuery, QueueConfigurations>
    {
        private readonly IQueueConfigurationRepository _QueueConfigurationRepository;

        public GetQueueConfigurationHandler(IQueueConfigurationRepository queueConfigurationRepository)
        {
            _QueueConfigurationRepository = queueConfigurationRepository;
        }
        public Task<QueueConfigurations> Handle(GetQueueConfigurationQuery request, CancellationToken cancellationToken)
        {
            return _QueueConfigurationRepository.Get(request.ID);
        }
    }
    public class GetQueueConfigurationQuery:IRequest<QueueConfigurations>
    {
        public int ID { get; set; }
    }
}
