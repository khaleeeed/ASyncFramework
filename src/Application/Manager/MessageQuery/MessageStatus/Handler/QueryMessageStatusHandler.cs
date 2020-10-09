using ASyncFramework.Domain.Interface;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageQuery.MessageStatus.Handler
{
    public class QueryMessageStatusHandler : IRequestHandler<MessageStatusQuery, object>
    {
        private readonly IASyncFrameworkInfrastructureRepository _repository;

        public QueryMessageStatusHandler(IASyncFrameworkInfrastructureRepository repository)
        {
            _repository = repository;
        }

        public Task<object> Handle(MessageStatusQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetMessageStatus(request.ReferenceNumber,request.From);
        }
    }
    public class MessageStatusQuery : IRequest<object>
    {
        public string ReferenceNumber { get; set; }
        public int From { get; set; }
    }
}