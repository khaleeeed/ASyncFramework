using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Systems.Command.UpdateSystem
{
    public class UpdateSystemCommandHandler : IRequestHandler<UpdateSystemCommand, Result>
    {
        private readonly ISystemRepository _SystemRepository;
        public UpdateSystemCommandHandler(ISystemRepository systemRepository)
        {
            _SystemRepository = systemRepository;
        }

        public async Task<Result> Handle(UpdateSystemCommand request, CancellationToken cancellationToken)
        {
            var isUpdated = await _SystemRepository.Update(request.SystemCode, request.EnSystemName, request.ArSystemName, request.IsActive, request.HasCustomQueue);


            if (!isUpdated)
                return new Result(false, new List<string> { "Data not updated" });

            return new Result(true, null);
        }
    }

    public class UpdateSystemCommand:IRequest<Result>
    {
        public int SystemCode { get; set; }
        public string EnSystemName { get; set; }
        public string ArSystemName { get; set; }
        public bool IsActive { get; set; }
        public bool HasCustomQueue { get; set; }
    }
}
