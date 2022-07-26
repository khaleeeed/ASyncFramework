using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Report.Query.GetMessageInProcessCountBySystemHandler
{
    public class GetMessageInProcessCountBySystemHandler : IRequestHandler<MessageInProcessCountBySystemQuery, long>
    {
        private readonly INotificationRepository _NotificationRepository;

        public GetMessageInProcessCountBySystemHandler(INotificationRepository notificationRepository)
        {
            _NotificationRepository = notificationRepository;
        }

        public Task<long> Handle(MessageInProcessCountBySystemQuery request, CancellationToken cancellationToken)
        {
            var total = _NotificationRepository.CountInProcessForSystem(request.from, request.to, request.systemCode);
            return total;
        }
    }

    public class MessageInProcessCountBySystemQuery:IRequest<long>
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string systemCode { get; set; }
    }
}