using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Systems.Query.GetSystemQuery
{
    public class GetSystemHandler : IRequestHandler<GetSystemQuery, SystemEntity>
    {
        private readonly ISystemRepository _SystemRepository;

        public GetSystemHandler(ISystemRepository systemRepository)
        {
            _SystemRepository = systemRepository;
        }

        public Task<SystemEntity> Handle(GetSystemQuery request, CancellationToken cancellationToken)
        {
            var system= _SystemRepository.GetSystem(request.SystemCode);
            return Task.FromResult(system);
        }
    }

    public class GetSystemQuery : IRequest<SystemEntity>
    {
        public string SystemCode { get; set; }
    }
}