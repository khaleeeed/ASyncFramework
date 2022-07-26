using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetTargetFailureCountHandler
{
    public class GetTargetFailureCountHandler : IRequestHandler<TargetFailureCountQuery, long>
    {
        private readonly ITargetFailuerRepository _TargetFailuerRepository;

        public GetTargetFailureCountHandler(ITargetFailuerRepository targetFailuerRepository)
        {
            _TargetFailuerRepository = targetFailuerRepository;
        }

        public async Task<long> Handle(TargetFailureCountQuery request, CancellationToken cancellationToken)
        {
            var total = await _TargetFailuerRepository.CountForAdmin(request.from, request.to);
            return total;
        }
    }

    public class TargetFailureCountQuery:IRequest<long>
    {
        public DateTime to { get; set; }
        public DateTime from { get; set; }
    }
}
