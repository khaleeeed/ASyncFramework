using ASyncFramework.Domain.Model.Response;
using MediatR;
using ASyncFramework.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;
using ASyncFramework.Domain.Interface.Repository;
using System.Linq;

namespace ASyncFramework.Application.Manager.Service.Query.GetAllService
{
    public class GetAllServiceHandler:IRequestHandler<GetAllServiceQuery, GenericDocumentResponse<ServiceEntity>>
    {
        private readonly IServiceRepository _ServiceRepository;

        public GetAllServiceHandler(IServiceRepository serviceRepository)
        {
            _ServiceRepository = serviceRepository;
        }

        public async Task<GenericDocumentResponse<ServiceEntity>> Handle(GetAllServiceQuery request, CancellationToken cancellationToken)
        {
            var services = await _ServiceRepository.GetAll(request.SystemCode);
            var count = services.Count();
            return new GenericDocumentResponse<ServiceEntity>
            {
                data = services,
                recordsTotal = count,
                recordsFiltered = count        
            };
        }
    }
    public class GetAllServiceQuery:IRequest<GenericDocumentResponse<ServiceEntity>>
    {
        public string SystemCode { get; set; }
    }
}
