using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Query.CallBackFailuerMessagesQuery.Handler
{
    public class CallBackFailuerMessagesQueryHandler : IRequestHandler<CallBackFailuerMessagesQuery, GenericDocumentResponse<CallBackFailuerDocument>>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;

        public CallBackFailuerMessagesQueryHandler(ICallBackFailureRepository callBackFailureRepository)
        {
            _CallBackFailureRepository = callBackFailureRepository;
        }

        public async Task<GenericDocumentResponse<CallBackFailuerDocument>> Handle(CallBackFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<CallBackFailuerDocument> doc, long total) = await _CallBackFailureRepository.GetAllFaulierDocument(request.From);
            
            return new GenericDocumentResponse<CallBackFailuerDocument>()
            {
                Document = doc,
                Total=total
            };
        }
    }
    public class CallBackFailuerMessagesQuery : IRequest<GenericDocumentResponse<CallBackFailuerDocument>>
    {
        public int From { get; set; }
    }
}