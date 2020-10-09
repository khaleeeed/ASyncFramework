using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.ManagerNotification.Query.GetAllNotification.Handler
{
    public class GetAllNotificationQueryHandler : IRequestHandler<GetAllNotificationQuery, Domain.Model.Response.GenericDocumentResponse<NotificationDocument>>
    {
        private readonly ILUTNotificationRepository _Repository;

        public GetAllNotificationQueryHandler(ILUTNotificationRepository repository)
        {
            _Repository = repository;
        }

        public async Task<GenericDocumentResponse<NotificationDocument>> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<NotificationDocument> doc, long total) = await _Repository.GetAllDocument(request.From);
            
            return new GenericDocumentResponse<NotificationDocument>
            {
                Document = doc,
                Total = total,
                Message = "OK"
            };            
        }
    }
    public class GetAllNotificationQuery:IRequest<GenericDocumentResponse<NotificationDocument>>
    {
        public int From { get; set; }
    }
}