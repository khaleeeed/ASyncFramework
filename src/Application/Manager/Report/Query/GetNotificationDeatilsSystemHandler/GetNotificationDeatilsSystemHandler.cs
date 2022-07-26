using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetNotificationDeatilsSystemHandler
{
    public class GetNotificationDeatilsSystemHandler : IRequestHandler<GetNotificationDeatilsSystemQuery, GenericDocumentResponse<NotificationEntity>>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetNotificationDeatilsSystemHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }

        public async Task<GenericDocumentResponse<NotificationEntity>> Handle(GetNotificationDeatilsSystemQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<NotificationEntity> doc, long total) = await _NotificationRepository.DetailsForSystem(request.From, request.FromDate, request.ToDate,request.SystemCode);

            return new GenericDocumentResponse<NotificationEntity>()
            {
                data = doc,
                recordsTotal = total,
                recordsFiltered = total
            };
        }
    }

    public class GetNotificationDeatilsSystemQuery : IRequest<GenericDocumentResponse<NotificationEntity>>
    {
        public int From { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SystemCode { get; set; }

    }
}
