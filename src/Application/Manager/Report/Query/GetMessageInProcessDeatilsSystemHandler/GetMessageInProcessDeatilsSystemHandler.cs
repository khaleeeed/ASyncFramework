using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetMessageInProcessDeatilsSystemHandler
{
    public class GetMessageInProcessDeatilsSystemHandler : IRequestHandler<GetMessageInProcessDeatilsSystemQuery, GenericDocumentResponse<NotificationEntity>>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetMessageInProcessDeatilsSystemHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }

        public async Task<GenericDocumentResponse<NotificationEntity>> Handle(GetMessageInProcessDeatilsSystemQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<NotificationEntity> doc, long total) = await _NotificationRepository.DetailsInProcessForSystem(request.From, request.FromDate, request.ToDate,request.SystemCode);

            return new GenericDocumentResponse<NotificationEntity>()
            {
                data = doc,
                recordsTotal = total,
                recordsFiltered = total
            };
        }
    }

    public class GetMessageInProcessDeatilsSystemQuery : IRequest<GenericDocumentResponse<NotificationEntity>>
    {
        public int From { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SystemCode { get; set; }


    }
}
