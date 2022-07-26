using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetMessageInProcessCountHandler
{
    public class GetMessageInProcessCountHandler : IRequestHandler<MessageInProcessCountQuery, long>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetMessageInProcessCountHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }

        public async Task<long> Handle(MessageInProcessCountQuery request, CancellationToken cancellationToken)
        {
            var total= await _NotificationRepository.CountInProcessForAdmin(request.from, request.to);
            return total;
        }
    }

    public class MessageInProcessCountQuery:IRequest<long>
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
