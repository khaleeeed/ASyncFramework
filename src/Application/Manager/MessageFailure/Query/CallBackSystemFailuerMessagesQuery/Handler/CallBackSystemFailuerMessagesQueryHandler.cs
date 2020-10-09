using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Query.CallBackSystemFailuerMessagesQuery.Handler
{
    public class CallBackSystemFailuerMessagesQueryHandler : IRequestHandler<CallBackSystemFailuerMessagesQuery, GenericDocumentResponse<CallBackFailuerDocument>>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;

        public CallBackSystemFailuerMessagesQueryHandler(ICallBackFailureRepository callBackFailureRepository)
        {
            _CallBackFailureRepository = callBackFailureRepository;
        }

        public async Task<GenericDocumentResponse<CallBackFailuerDocument>> Handle(CallBackSystemFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<CallBackFailuerDocument> doc, long total) = await _CallBackFailureRepository.GetAllFaulierDocumentBySystemCode(request.From,request.SystemCode);

            return new GenericDocumentResponse<CallBackFailuerDocument>()
            {
                Document = doc,
                Total = total
            };
        }
    }
    public class CallBackSystemFailuerMessagesQuery : IRequest<GenericDocumentResponse<CallBackFailuerDocument>>
    {
        public int From { get; set; }
        public string SystemCode { get; set; }
    }
}
