using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetTargetFailureCountBySystemHandler
{
    public class GetTargetFailureCountBySystemHandler : IRequestHandler<TargetFailureCountBySystemQuery, long>
    {
        private readonly ITargetFailuerRepository _TargetFailuerRepository;

        public GetTargetFailureCountBySystemHandler(ITargetFailuerRepository targetFailuerRepository)
        {
            _TargetFailuerRepository = targetFailuerRepository;
        }

        public async Task<long> Handle(TargetFailureCountBySystemQuery request, CancellationToken cancellationToken)
        {
           var total = await _TargetFailuerRepository.CountForSystem(request.from, request.to, request.systemCode);
           return total;
        }
    }
    public class TargetFailureCountBySystemQuery:IRequest<long>
    {
        public DateTime to { get; set; }
        public DateTime from { get; set; }
        public string systemCode { get; set; }
    }
}
