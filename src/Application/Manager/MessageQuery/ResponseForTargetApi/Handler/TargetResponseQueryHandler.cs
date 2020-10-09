using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageQuery.ResponseForTargetApi.Handler
{
    public class TargetResponseQueryHandler : IRequestHandler<TargetResponseQuery, object>
    {
        private readonly IASyncFrameworkInfrastructureRepository _repository;

        public TargetResponseQueryHandler(IASyncFrameworkInfrastructureRepository repository)
        {
            _repository = repository;
        }

        public Task<object> Handle(TargetResponseQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetTargetResponse(request.ReferenceNumber, request.From);
        }
    }
    public class TargetResponseQuery : IRequest<object>
    {
        public string ReferenceNumber { get; set; }
        public int From { get; set; }
    }
}
