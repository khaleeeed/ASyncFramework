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

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Query.TargetSystemFailuerMessagesQuery.Handler
{
    public class TargetSystemFailuerMessagesQueryHandler : IRequestHandler<TargetSystemFailuerMessagesQuery, GenericDocumentResponse<TargetFailuerEntity>>
    {
        private readonly ITargetFailuerRepository _TargetFailureRepository;

        public TargetSystemFailuerMessagesQueryHandler(ITargetFailuerRepository targetFailureRepository)
        {
            _TargetFailureRepository = targetFailureRepository;
        }

        public async Task<GenericDocumentResponse<TargetFailuerEntity>> Handle(TargetSystemFailuerMessagesQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<TargetFailuerEntity> doc, long total) = await _TargetFailureRepository.GetAllFaulierDocumentBySystemCode(request.From,request.SystemCode);

            return new GenericDocumentResponse<TargetFailuerEntity>()
            {
                data = doc,
                recordsFiltered = total,
                recordsTotal=total
            };
        }
    }
    public class TargetSystemFailuerMessagesQuery : IRequest<GenericDocumentResponse<TargetFailuerEntity>>
    {
        public int From { get; set; }
        public string SystemCode { get; set; }
    }
}