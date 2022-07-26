using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Account.Command.CreateNewTokenFromRefreshTokenCommandHandler
{
    public class CreateNewTokenCommandHandler : IRequestHandler<CreateNewTokenCommand, GenericServiceResponse<TokenResponse>>
    {
        private readonly IIdentityRepository _IdentityRepository;
        private readonly ITokenRepository _TokenRepository;

        public CreateNewTokenCommandHandler(IIdentityRepository identityRepository, ITokenRepository tokenRepository)
        {
            _IdentityRepository = identityRepository;
            _TokenRepository = tokenRepository;
        }

        public async Task<GenericServiceResponse<TokenResponse>> Handle(CreateNewTokenCommand request, CancellationToken cancellationToken)
        {
            var userName = await _TokenRepository.ValidateRefreshToken(request.RefreshToken);
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new GenericServiceResponse<TokenResponse> { IsSuccessful = false, StatusCode = HttpStatusCode.Unauthorized, ResponseMessage = "wrong or expire RefreshToken" };
            }

            var user = await _IdentityRepository.FindByNameAsync(userName);

            (string token, DateTime expireDate)  = await _TokenRepository.GenerateToken(user);
            (string refreshToken, DateTime expireDateRefreshToken)  = await _TokenRepository.GenerateRefreshToken(userName);
           
            return new GenericServiceResponse<TokenResponse> { Data = new TokenResponse { Token = token, CreatedBy = userName, RefreshToken = refreshToken,ExpireTime=expireDate }, IsSuccessful = true, ResponseMessage = "Create", StatusCode = HttpStatusCode.OK };
        }
    }

    public class CreateNewTokenCommand : IRequest<GenericServiceResponse<TokenResponse>>
    {
        public string RefreshToken { get; set; }
    }
}