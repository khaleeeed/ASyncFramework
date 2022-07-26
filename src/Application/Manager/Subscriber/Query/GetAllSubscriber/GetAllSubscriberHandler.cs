using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Subscriber.Query.GetAllSubscriber
{
    public class GetAllSubscriberHandler : IRequestHandler<GetAllSubscriberQuery, GenericDocumentResponse<SubscriberEntity>>
    {
        private readonly ISubscriberRepository _SubscriberRepository;

        public GetAllSubscriberHandler(ISubscriberRepository subscriberRepository)
        {
            _SubscriberRepository = subscriberRepository;
        }

        public async Task<GenericDocumentResponse<SubscriberEntity>> Handle(GetAllSubscriberQuery request, CancellationToken cancellationToken)
        {
            var response = await _SubscriberRepository.GetAll();
            var count = response.Count();
            return new GenericDocumentResponse<SubscriberEntity>
            {
                data = response,
                recordsFiltered= count,
                recordsTotal= count
            };
        }
    }
    public class GetAllSubscriberQuery:IRequest<GenericDocumentResponse<SubscriberEntity>>
    {

    }
}
