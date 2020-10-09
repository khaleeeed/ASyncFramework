using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.ManagerNotification.Command.CreateNewNotificationCommand.Handler
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result>
    {
        private readonly ILUTNotificationRepository _NotificationRepository;
        private readonly IReferenceNumberService _ReferenceNumberService;
        public CreateNotificationCommandHandler(ILUTNotificationRepository notificationRepository,IReferenceNumberService referenceNumberService)
        {
            _NotificationRepository = notificationRepository;
            _ReferenceNumberService = referenceNumberService;
        }

        public async Task<Result> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var notificationFields = new Domain.Documents.NotificationFields
            {
                ArName = request.ArName,
                CreationDate = DateTime.Now,
                Description = request.Description,
                Name = request.Name,
                ReferenceNumber = _ReferenceNumberService.ReferenceNumber,
                Type = request.Type
            };

            var result = await _NotificationRepository.AddNotification(request.SystemCode,request.ContentType, request.SamplePayload, notificationFields);
            if (result)
                return new Result(true, null) {  ReferenceNumber=_ReferenceNumberService.ReferenceNumber};

            return new Result(false, new string[] { "data not insert" });
        }
    }
    public class CreateNotificationCommand:IRequest<Result>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public string Description { get; set; }
        public string SystemCode { get; set; }
        public string SamplePayload { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
    }
}
