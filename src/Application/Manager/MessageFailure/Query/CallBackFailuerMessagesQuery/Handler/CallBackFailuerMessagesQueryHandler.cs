using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Entities;
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
    public class CallBackFailuerMessagesQueryHandler : IRequestHandler<CallBackFailuerMessagesQuery, GenericDocumentResponse<CallBackFailuerEntity>>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;

        public CallBackFailuerMessagesQueryHandler(ICallBackFailureRepository callBackFailureRepository)
        {
            _CallBackFailureRepository = callBackFailureRepository;
        }

        public async Task<GenericDocumentResponse<CallBackFailuerEntity>> Handle(CallBackFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<CallBackFailuerEntity> doc, long total) = await _CallBackFailureRepository.GetAllFaulierDocument(request.From);
            
            return new GenericDocumentResponse<CallBackFailuerEntity>()
            {
                data = doc,
                recordsTotal=total,
                recordsFiltered=total
            };
        }
    }
    public class CallBackFailuerMessagesQuery : IRequest<GenericDocumentResponse<CallBackFailuerEntity>>
    {
        public int From { get; set; }
    }
}