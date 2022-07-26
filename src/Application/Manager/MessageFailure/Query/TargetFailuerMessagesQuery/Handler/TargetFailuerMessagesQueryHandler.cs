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

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetFailuerMessagesQuery.Handler
{
    public class TargetFailuerMessagesQueryHandler : IRequestHandler<TargetFailuerMessagesQuery, GenericDocumentResponse<TargetFailuerEntity>>
    {
        private readonly ITargetFailuerRepository _TargetFailureRepository;

        public TargetFailuerMessagesQueryHandler(ITargetFailuerRepository targetFailureRepository)
        {
            _TargetFailureRepository = targetFailureRepository;
        }

        public async Task<GenericDocumentResponse<TargetFailuerEntity>> Handle(TargetFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<TargetFailuerEntity> doc, long total) = await _TargetFailureRepository.GetAllFaulierDocument(request.From);

            return new GenericDocumentResponse<TargetFailuerEntity>()
            {
                data = doc,
                recordsTotal = total,
                recordsFiltered=total
            };
        }
    }
    public class TargetFailuerMessagesQuery : IRequest<GenericDocumentResponse<TargetFailuerEntity>>
    {
        public int From { get; set; }
    }
}