using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetFinshNotificationCountHandler
{
    public class GetFinshNotificationCountHandler : IRequestHandler<GetFinshNotificationCountQuery, long>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetFinshNotificationCountHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }
        public async Task<long> Handle(GetFinshNotificationCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _NotificationRepository.CountFinshForAdmin(request.from, request.to);
            return count;
        }
    }

    public class GetFinshNotificationCountQuery:IRequest<long>
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }         
}
