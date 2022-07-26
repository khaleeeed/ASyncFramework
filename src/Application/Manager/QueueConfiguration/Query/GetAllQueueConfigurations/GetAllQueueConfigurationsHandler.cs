using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.QueueConfiguration.Query.GetAllQueueConfigurations
{
    public class GetAllQueueConfigurationsHandler : IRequestHandler<GetAllQueueConfigurationsQuery, GenericDocumentResponse<QueueConfigurations>>
    {
        private readonly IQueueConfigurationRepository _QueueConfigurationRepository;

        public GetAllQueueConfigurationsHandler(IQueueConfigurationRepository queueConfigurationRepository)
        {
            _QueueConfigurationRepository = queueConfigurationRepository;
        }

        public async Task<GenericDocumentResponse<QueueConfigurations>> Handle(GetAllQueueConfigurationsQuery request, CancellationToken cancellationToken)
        {
            var response = await _QueueConfigurationRepository.GetAll();
            var count = response.Count();
            return new GenericDocumentResponse<QueueConfigurations>
            {
                data = response,
                recordsFiltered = count,
                recordsTotal = count
            };
        }
    }
    public class GetAllQueueConfigurationsQuery:IRequest<GenericDocumentResponse<QueueConfigurations>>
    {

    }
}
