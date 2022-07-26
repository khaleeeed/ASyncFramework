using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Account.Command.RemoveUserCommandHandler
{
    public class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, Result>
    {
        private readonly IIdentityRepository _IdentityRepository;

        public RemoveUserCommandHandler(IIdentityRepository identityRepository)
        {
            _IdentityRepository = identityRepository;
        }
        public async Task<Result> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _IdentityRepository.FindByNameAsync(request.UserName);
            if (user is null)
            {
                return new Result(false, new string[] { "user not found" });
            }

            _ = _IdentityRepository.RemoveRole(user, $"{user.Roles}-{user.System}");
            return new Result(true, null);
        }
    }

    public class RemoveUserCommand:IRequest<Result>
    {
        public string UserName { get; set; }
    }
}
