using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageQuery.SearchByContentBodyForAdmin.Handler
{
    public class SearchByContentBodyForAdminHandler : IRequestHandler<SearchByContentBodyForAdminQuery, object>
    {
        private readonly IASyncFrameworkInfrastructureRepository _repository;

        public SearchByContentBodyForAdminHandler(IASyncFrameworkInfrastructureRepository repository)
        {
            _repository = repository;
        }
        public Task<object> Handle(SearchByContentBodyForAdminQuery request, CancellationToken cancellationToken)
        {
           return _repository.GetMessageByContentBodyForAdmin(request.FieldName, request.FieldValue, request.From);
        }
    }
    public class SearchByContentBodyForAdminQuery:IRequest<object>
    {
        public string FieldValue { get; set; }
        public string FieldName { get; set; }
        public int From { get; set; }
    }
}
