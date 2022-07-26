using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Account.Command
{
    public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, GenericServiceResponse<TokenResponse>>
    {
        private readonly IIdentityRepository _IdentityRepository;
        private readonly ITokenRepository _TokenRepository;

        public CreateTokenCommandHandler(IIdentityRepository identityRepository, ITokenRepository tokenRepository)
        {
            _IdentityRepository = identityRepository;
            _TokenRepository = tokenRepository;
        }

        public async Task<GenericServiceResponse<TokenResponse>> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
        {
            AsyncUser user = await _IdentityRepository.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new GenericServiceResponse<TokenResponse> { IsSuccessful = false, ResponseMessage = "User not found" , StatusCode=HttpStatusCode.NotFound };
            }

            if (!await _IdentityRepository.CheckPasswordAsync(request.UserName, request.Password))
            {
                return new GenericServiceResponse<TokenResponse> { IsSuccessful = false, ResponseMessage = "Wrong password" , StatusCode=HttpStatusCode.Unauthorized };
            }

            if(string.IsNullOrWhiteSpace(user.Roles))
            {
                return new GenericServiceResponse<TokenResponse> { IsSuccessful = false, ResponseMessage = "Unauthorized user", StatusCode = HttpStatusCode.Unauthorized };
            }
            if (user.Roles== "Pending")
            {
                return new GenericServiceResponse<TokenResponse> { IsSuccessful = false, ResponseMessage = "Pending", StatusCode = HttpStatusCode.Unauthorized };
            }

            (string token, DateTime expireDate) = await _TokenRepository.GenerateToken(user);
            (string refreshToken, DateTime expireDateRefreshToken) = await _TokenRepository.GenerateRefreshToken(request.UserName);


            return new GenericServiceResponse<TokenResponse> { Data = new TokenResponse {  Token = token, CreatedBy=request.UserName,RefreshToken= refreshToken,ExpireTime=expireDate }, IsSuccessful = true, ResponseMessage = "Create", StatusCode = HttpStatusCode.OK };
        }
    }


    public class CreateTokenCommand:IRequest<GenericServiceResponse<TokenResponse>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}