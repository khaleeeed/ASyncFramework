using ASyncFramework.Domain.Interface;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageQuery.SearchByContentBodyForSystem.Handler
{
    public class SearchByContentBodyForSystemHandler : IRequestHandler<SearchByContentBodyForSystemQuery, object>
    {
        private readonly IASyncFrameworkInfrastructureRepository _repository;

        public SearchByContentBodyForSystemHandler(IASyncFrameworkInfrastructureRepository repository)
        {
            _repository = repository;
        }
        public Task<object> Handle(SearchByContentBodyForSystemQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetMessageByContentBodyForSystem(request.FieldName, request.FieldValue, request.From,request.SystemCode);
        }
    }
    public class SearchByContentBodyForSystemQuery : IRequest<object>
    {
        public string SystemCode { get; set; }
        public string FieldValue { get; set; }
        public string FieldName { get; set; }
        public int From { get; set; }
    }
}
