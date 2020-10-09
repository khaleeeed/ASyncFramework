using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetFailuerMessagesQuery.Handler
{
    public class TargetFailuerMessagesQueryHandler : IRequestHandler<TargetFailuerMessagesQuery, GenericDocumentResponse<TargetFailuerDocument>>
    {
        private readonly ITargetFailuerRepository _TargetFailureRepository;

        public TargetFailuerMessagesQueryHandler(ITargetFailuerRepository targetFailureRepository)
        {
            _TargetFailureRepository = targetFailureRepository;
        }

        public async Task<GenericDocumentResponse<TargetFailuerDocument>> Handle(TargetFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<TargetFailuerDocument> doc, long total) = await _TargetFailureRepository.GetAllFaulierDocument(request.From);

            return new GenericDocumentResponse<TargetFailuerDocument>()
            {
                Document = doc,
                Total = total
            };
        }
    }
    public class TargetFailuerMessagesQuery : IRequest<GenericDocumentResponse<TargetFailuerDocument>>
    {
        public int From { get; set; }
    }
}