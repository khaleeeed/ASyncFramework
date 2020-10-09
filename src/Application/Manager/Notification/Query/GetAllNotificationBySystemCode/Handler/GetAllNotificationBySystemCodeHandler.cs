using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.ManagerNotification.Query.GetAllNotificationBySystemCode.Handler
{
    public class GetAllNotificationBySystemCodeHandler : IRequestHandler<GetAllNotificationBySystemCode, GenericDocumentResponse<NotificationDocument>>
    {
        private readonly ILUTNotificationRepository _Repository;

        public GetAllNotificationBySystemCodeHandler(ILUTNotificationRepository repository)
        {
            _Repository = repository;
        }
        public async Task<GenericDocumentResponse<NotificationDocument>> Handle(GetAllNotificationBySystemCode request, CancellationToken cancellationToken)
        {
            (IEnumerable<NotificationDocument> doc, long total) = await _Repository.GetDocumentBySystemCode(request.From, request.SystemCode);
            return new GenericDocumentResponse<NotificationDocument>
            {
                Document = doc,
                Total = total,
                Message = "Ok"
            };
        }
    }

    public class GetAllNotificationBySystemCode: IRequest<GenericDocumentResponse<NotificationDocument>>
    {
        public int From { get; set; }
        public string SystemCode { get; set; }
    }
}