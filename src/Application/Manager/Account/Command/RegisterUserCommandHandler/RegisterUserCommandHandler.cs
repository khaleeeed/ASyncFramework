using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Account.Command.RegisterUserCommandHandler
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IIdentityRepository _IdentityRepository;
        private readonly ISendEmailService _SendEmailService;
        private readonly ITokenRepository _TokenRepository;

        public RegisterUserCommandHandler(IIdentityRepository identityRepository, ISendEmailService sendEmailService, ITokenRepository tokenRepository)
        {
            _IdentityRepository = identityRepository;
            _SendEmailService = sendEmailService;
            _TokenRepository = tokenRepository;
        }

        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _IdentityRepository.FindByNameAsync(request.UserName);
            if (user is null)
            {
                return new Result(false, new string[] { "user not found" });
            }
            var token = await _TokenRepository.GenerateRegisterToken(request.UserName, request.SystemCode);

            _ = _SendEmailService.SendConfirmationEmail(request.UserName, request.SystemName, token);
            _ = _IdentityRepository.AddRole(user, $"Pending-0");
            return new Result(true, null);
        }
    }
    public class RegisterUserCommand:IRequest<Result>
    {
        public string UserName { get; set; }
        public string SystemCode { get; set; }
        public string SystemName { get; set; }
    }
}