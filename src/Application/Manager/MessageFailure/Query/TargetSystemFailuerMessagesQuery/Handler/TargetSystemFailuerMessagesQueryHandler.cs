using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetSystemFailuerMessagesQuery.Handler
{
    public class TargetSystemFailuerMessagesQueryHandler : IRequestHandler<TargetSystemFailuerMessagesQuery, GenericDocumentResponse<TargetFailuerDocument>>
    {
        private readonly ITargetFailuerRepository _TargetFailureRepository;

        public TargetSystemFailuerMessagesQueryHandler(ITargetFailuerRepository targetFailureRepository)
        {
            _TargetFailureRepository = targetFailureRepository;
        }

        public async Task<GenericDocumentResponse<TargetFailuerDocument>> Handle(TargetSystemFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<TargetFailuerDocument> doc, long total) = await _TargetFailureRepository.GetAllFaulierDocumentBySystemCode(request.From,request.SystemCode);

            return new GenericDocumentResponse<TargetFailuerDocument>()
            {
                Document = doc,
                Total = total
            };
        }
    }
    public class TargetSystemFailuerMessagesQuery : IRequest<GenericDocumentResponse<TargetFailuerDocument>>
    {
        public int From { get; set; }
        public string SystemCode { get; set; }
    }
}