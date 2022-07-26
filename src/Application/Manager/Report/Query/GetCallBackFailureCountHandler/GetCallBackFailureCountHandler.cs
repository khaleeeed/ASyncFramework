using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetCallBackFailureCountHandler
{
    public class GetCallBackFailureCountHandler : IRequestHandler<CallBackFailureCountQuery, long>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;

        public GetCallBackFailureCountHandler(ICallBackFailureRepository callBackFailureRepository)
        {
            _CallBackFailureRepository = callBackFailureRepository;
        }

        public async Task<long> Handle(CallBackFailureCountQuery request, CancellationToken cancellationToken)
        {
            var total = await _CallBackFailureRepository.CountForAdmin(request.from, request.to);
            return total;
        }
    }

    public class CallBackFailureCountQuery:IRequest<long>
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
