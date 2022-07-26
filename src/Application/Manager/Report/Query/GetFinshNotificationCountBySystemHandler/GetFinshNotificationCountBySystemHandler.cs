using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetFinshNotificationCountBySystemHandler
{
    public class GetFinshNotificationCountBySystemHandler : IRequestHandler<GetFinshNotificationCountBySystemQuery, long>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetFinshNotificationCountBySystemHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }
        public async Task<long> Handle(GetFinshNotificationCountBySystemQuery request, CancellationToken cancellationToken)
        {
            var count= await _NotificationRepository.CountFinshForSystem(request.from, request.to, request.systemCode);
            return count;
        }
    }

    public class GetFinshNotificationCountBySystemQuery:IRequest<long>
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string systemCode { get; set; }
    }
}
