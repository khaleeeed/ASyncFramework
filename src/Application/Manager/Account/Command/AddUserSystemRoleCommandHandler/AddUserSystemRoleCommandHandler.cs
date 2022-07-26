using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Account.Command.AddUserSystemRoleCommandHandler
{
    public class AddUserSystemRoleCommandHandler : IRequestHandler<AddUserSystemRoleCommand, Result>
    {
        private readonly IIdentityRepository _IdentityRepository;
        
        public AddUserSystemRoleCommandHandler(IIdentityRepository identityRepository)
        {
            _IdentityRepository = identityRepository;
        }
        public async Task<Result> Handle(AddUserSystemRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _IdentityRepository.FindByNameAsync(request.UserName);
            if (user is null)
            {
                return new Result(false, new string[] { "user not found" });
            }
            if (!string.IsNullOrWhiteSpace(user.Roles) || !string.IsNullOrWhiteSpace(user.System))
                return new Result(false, new string[] { "user already exist" });

            _ = _IdentityRepository.AddRole(user, $"System-{request.SystemCode}");
            return new Result(true, null);
        }
    }
    public class AddUserSystemRoleCommand:IRequest<Result>
    {
        public string UserName { get; set; }
        public string SystemCode { get; set; }
    }
}