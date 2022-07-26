using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Account.Query.SystemsQueryHandler
{
    public class SystemsQueryHandler : IRequestHandler<SystemsQuery, List<MOJSystemResponse>>
    {
        private readonly IMojSystemService _MojSystemService;

        public SystemsQueryHandler(IMojSystemService mojSystemService)
        {
            _MojSystemService = mojSystemService;
        }

        public async Task<List<MOJSystemResponse>> Handle(SystemsQuery request, CancellationToken cancellationToken)
        {
            var mojSystem = await _MojSystemService.GetAllSystems();
            return mojSystem.Data;
        }
    }

    public class SystemsQuery:IRequest<List<MOJSystemResponse>>
    {

    }
}
