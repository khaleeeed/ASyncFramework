using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetNotificationCountBySystemHandler
{
    public class GetNotificationCountBySystemHandler : IRequestHandler<NotificationCountBySystemQuery, long>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetNotificationCountBySystemHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }

        public Task<long> Handle(NotificationCountBySystemQuery request, CancellationToken cancellationToken)
        {
            var total = _NotificationRepository.CountForSystem(request.from, request.to, request.systemCode);
            return total;
        }
    }

    public class NotificationCountBySystemQuery:IRequest<long>
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string systemCode { get; set; }
    }
}
