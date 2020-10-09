using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Interface.Repository;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.SystemNotification.Command.CreateSystemCommand.Handler
{
    public class CreateSystemCommandHandler : IRequestHandler<CreateSystemCommand, Result>
    {
        private readonly ILUTNotificationRepository _Repository;

        public CreateSystemCommandHandler(ILUTNotificationRepository repository)
        {
            _Repository = repository;
        }

        public async Task<Result> Handle(CreateSystemCommand request, CancellationToken cancellationToken)
        {
            var systemCode = await _Repository.GetNewSystemCode();

            var system = new Domain.Documents.NotificationDocument
            {
                Fields = new List<Domain.Documents.NotificationFields>(),
                SystemArName = request.SystemArName,
                SystemName = request.SystemName,
                SystemCode = systemCode
            };

            var commandResult = await _Repository.Add(system);

            if (commandResult)
                return new Result(true, null) { ReferenceNumber = systemCode.ToString() };

            return new Result(false, new string[] { "data not insert" });
        }
    }

    public class CreateSystemCommand:IRequest<Result>
    {
        public string SystemName { get; set; }
        public string SystemArName { get; set; }
    }
}