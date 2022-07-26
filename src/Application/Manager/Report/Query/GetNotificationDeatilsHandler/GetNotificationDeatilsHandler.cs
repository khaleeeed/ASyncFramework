using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetNotificationDeatilsHandler
{
    public class GetNotificationDetailsHandler : IRequestHandler<GetNotificationDetailsQuery, GenericDocumentResponse<NotificationEntity>>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetNotificationDetailsHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }

        public async Task<GenericDocumentResponse<NotificationEntity>> Handle(GetNotificationDetailsQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<NotificationEntity> doc, long total) = await _NotificationRepository.DetailsForAdmin(request.From,request.FromDate,request.ToDate);

            return new GenericDocumentResponse<NotificationEntity>()
            {
                data = doc,
                recordsTotal = total,
                recordsFiltered = total
            };
        }
    }

    public class GetNotificationDetailsQuery: IRequest<GenericDocumentResponse<NotificationEntity>>
    {
        public int From { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
}
