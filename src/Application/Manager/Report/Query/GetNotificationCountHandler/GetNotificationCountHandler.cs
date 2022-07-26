using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetNotificationCountHandler
{
    public class GetNotificationCountHandler : IRequestHandler<GetNotificationCountQuery, long>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetNotificationCountHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }

        public Task<long> Handle(GetNotificationCountQuery request, CancellationToken cancellationToken)
        {
            var total = _NotificationRepository.CountForAdmin(request.from, request.to);
            return total;
        }
    }

    public class GetNotificationCountQuery:IRequest<long>
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
