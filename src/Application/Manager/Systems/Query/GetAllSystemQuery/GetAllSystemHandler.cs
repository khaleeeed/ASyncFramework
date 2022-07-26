using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Systems.Query.GetAllSystemQuery
{

    public class GetAllSystemQueryHandler : IRequestHandler<GetAllSystemQuery, GenericDocumentResponse<SystemEntity>>
    {
        private readonly ISystemRepository _SystemRepository;

        public GetAllSystemQueryHandler(ISystemRepository systemRepository)
        {
            _SystemRepository = systemRepository;
        }

        public async Task<GenericDocumentResponse<SystemEntity>> Handle(GetAllSystemQuery request, CancellationToken cancellationToken)
        {
            var data= await _SystemRepository.GetAll();
            var count = data.Count();

            return new GenericDocumentResponse<SystemEntity>
            {
                data=data,
                recordsFiltered = count,
                recordsTotal = count
            };
        }
    }

    public class GetAllSystemQuery : IRequest<GenericDocumentResponse<SystemEntity>>
    {

    }
}
