using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.MessageQuery.MessageStatus.Handler
{
    public class QueryMessageStatusHandler : IRequestHandler<QueryMessageStatus, object>
    {
        private IASyncFrameworkInfrastructureRepository _repository;

        public QueryMessageStatusHandler(IASyncFrameworkInfrastructureRepository repository)
        {
            _repository = repository;
        }

        public async Task<object> Handle(QueryMessageStatus request, CancellationToken cancellationToken)
        {
            var asss = await _repository.GetMessageStatus(request.ReferenceNumber);
            return  asss.obj;
        }
    }
    public class QueryMessageStatus : IRequest<object>
    {
        public string ReferenceNumber { get; set; }
    }
}