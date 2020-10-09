using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.MessageQuery.ResponseForCallBackApi.Handler
{
    public class CallBackResponseQueryHandler : IRequestHandler<CallBackResponseQuery, object>
    {
        private IASyncFrameworkInfrastructureRepository _repository;

        public CallBackResponseQueryHandler(IASyncFrameworkInfrastructureRepository repository)
        {
            _repository = repository;
        }

        public Task<object> Handle(CallBackResponseQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetCallBackResponse(request.ReferenceNumber, request.From);
        }
    }
    public class CallBackResponseQuery : IRequest<object>
    {
        public string ReferenceNumber { get; set; }
        public int From { get; set; }
    }
}
