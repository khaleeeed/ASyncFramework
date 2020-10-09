using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageQuery.SearchByCallBackUrl.Handler
{
    public class CallBackUrlQueryHandler : IRequestHandler<CallBackUrlQuery, object>
    {
        private readonly IASyncFrameworkInfrastructureRepository _repository;

        public CallBackUrlQueryHandler(IASyncFrameworkInfrastructureRepository repository)
        {
            _repository = repository;
        }

        public Task<object> Handle(CallBackUrlQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetMessageFromCallBackUrl(request.CallBackUrl, request.From);
        }

    }
    public class CallBackUrlQuery : IRequest<object>
    {
        public string CallBackUrl { get; set; }
        public int From { get; set; }
    }
}
