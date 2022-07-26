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

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Query.CallBackSystemFailuerMessagesQuery.Handler
{
    public class CallBackSystemFailuerMessagesQueryHandler : IRequestHandler<CallBackSystemFailuerMessagesQuery, GenericDocumentResponse<CallBackFailuerEntity>>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;

        public CallBackSystemFailuerMessagesQueryHandler(ICallBackFailureRepository callBackFailureRepository)
        {
            _CallBackFailureRepository = callBackFailureRepository;
        }

        public async Task<GenericDocumentResponse<CallBackFailuerEntity>> Handle(CallBackSystemFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<CallBackFailuerEntity> doc, long total) = await _CallBackFailureRepository.GetAllFaulierDocumentBySystemCode(request.From,request.SystemCode);

            return new GenericDocumentResponse<CallBackFailuerEntity>()
            {
                data = doc,
                recordsFiltered = total,
                recordsTotal=total
            };
        }
    }
    public class CallBackSystemFailuerMessagesQuery : IRequest<GenericDocumentResponse<CallBackFailuerEntity>>
    {
        public int From { get; set; }
        public string SystemCode { get; set; }
    }
}
