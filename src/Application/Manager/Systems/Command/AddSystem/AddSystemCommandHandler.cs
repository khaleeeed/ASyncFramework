using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Systems.Command.AddSystem
{
    public class AddSystemCommandHandler : IRequestHandler<AddSystemCommand, Result>
    {
        private readonly ISystemRepository _SystemRepository;
        public AddSystemCommandHandler(ISystemRepository systemRepository)
        {
            _SystemRepository = systemRepository;
        }
        public async Task<Result> Handle(AddSystemCommand request, CancellationToken cancellationToken)
        {
            var isAdd = await _SystemRepository.Add(new Domain.Entities.SystemEntity
            {
                ArSystemName = request.ArSystemName,
                EnSystemName = request.EnSystemName,
                HasCustomQueue = request.HasCustomQueue,
                IsActive = request.IsActive,
                SystemCode = request.SystemCode                
            });

            if (!isAdd)
            {
                return new Result(false, new List<string> { "Queue not add" });
            }
            return new Result(true, null);
        }
    }

    public class AddSystemCommand : IRequest<Result>
    {
        public string ArSystemName { get; set; }
        public string EnSystemName { get; set; }
        public bool HasCustomQueue { get; set; }
        public bool IsActive { get; set; }
        public int SystemCode { get; set; }
    }
}
