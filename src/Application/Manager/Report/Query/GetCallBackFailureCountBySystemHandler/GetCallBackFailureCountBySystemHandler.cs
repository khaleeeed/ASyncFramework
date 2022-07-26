using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetCallBackFailureCountBySystemHandler
{
    public class GetCallBackFailureCountBySystemHandler : IRequestHandler<CallBackFailureCountBySystemQuery, long>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;

        public GetCallBackFailureCountBySystemHandler(ICallBackFailureRepository callBackFailureRepository)
        {
            _CallBackFailureRepository = callBackFailureRepository;
        }

        public async Task<long> Handle(CallBackFailureCountBySystemQuery request, CancellationToken cancellationToken)
        {
             var total= await _CallBackFailureRepository.CountForSystem(request.from, request.to,request.systemCode);

            return total;
        }
    }

    public class CallBackFailureCountBySystemQuery : IRequest<long>
    {
        public DateTime to { get; set; }
        public DateTime from { get; set; }
        public string systemCode { get; set; }
    }
}
