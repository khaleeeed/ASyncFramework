using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Account.Command.ConfirmationAddUserCommandHandler
{
    public class ConfirmationAddUserCommandHandler : IRequestHandler<ConfirmationAddUserCommand, Result>
    {
        private readonly IIdentityRepository _IdentityRepository;
        private readonly ITokenRepository _TokenRepository;

        public ConfirmationAddUserCommandHandler(IIdentityRepository identityRepository, ITokenRepository tokenRepository)
        {
            _IdentityRepository = identityRepository;
            _TokenRepository = tokenRepository;
        }

        public async Task<Result> Handle(ConfirmationAddUserCommand request, CancellationToken cancellationToken)
        {
            (string userName, string systemCode) = await _TokenRepository.ValidateRegisterToken(request.Token);
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(systemCode))
            {
                return new Result(false, new string[] { "Error validate token" });
            }
            var user = await _IdentityRepository.FindByNameAsync(userName);
            if (user is null)
            {
                return new Result(false, new string[] { "user not found" });
            }
            _ = _IdentityRepository.AddRole(user, $"System-{systemCode}");

            return new Result(true, null);
        }
    }

    public class ConfirmationAddUserCommand:IRequest<Result>
    {
        public string Token { get; set; }
    }
}
