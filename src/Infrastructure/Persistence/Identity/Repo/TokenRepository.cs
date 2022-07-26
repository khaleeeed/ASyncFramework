using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.Identity.Repo
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IMojSystemService _mojSystemService;

        public TokenRepository(IMojSystemService mojSystemService)
        {
            _mojSystemService = mojSystemService;
        }

        public Task<(string token ,DateTime expireDate)> GenerateToken(AsyncUser user)
        {

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationJWT.Key));

            var systemName= _mojSystemService.GetSystemDetails(user.System);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Role,user.Roles),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim("SystemCode",user.System),
                
            };

            if (systemName != null)
            {
                authClaims.Add(new Claim("SystemName", systemName));
            }

            var expireDate = DateTime.Now.AddHours(2);

            var securityToken = new JwtSecurityToken(
                issuer: ConfigurationJWT.Issuer,
                audience: ConfigurationJWT.Aduince,
                expires: expireDate,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return Task.FromResult((token,expireDate));
        }

        public Task<(string token, DateTime expireDate)> GenerateRefreshToken(string userName)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationJWT.RefreshKey));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName)
            };

            var expireDate = DateTime.Now.AddHours(6);

            var securityToken = new JwtSecurityToken(
                issuer: ConfigurationJWT.Issuer,
                audience: ConfigurationJWT.Aduince,
                expires: expireDate,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return Task.FromResult((token, expireDate));
        }

        public Task<string> ValidateRefreshToken(string token)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationJWT.RefreshKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = ConfigurationJWT.Issuer,
                ValidAudience = ConfigurationJWT.Aduince,
                IssuerSigningKey = authSigningKey
            };

            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                ClaimsPrincipal claims = principal as ClaimsPrincipal;

                string name = claims.FindFirst(ClaimTypes.Name).Value;
                return Task.FromResult(name);
            }
            catch
            {
                return Task.FromResult(string.Empty);
            }
        }

        public Task<string> GenerateRegisterToken(string userName, string systemCode)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationJWT.RegisterUserKey));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim("systemCode",systemCode)
            };

            var securityToken = new JwtSecurityToken(
                issuer: ConfigurationJWT.Issuer,
                audience: ConfigurationJWT.Aduince,
                expires: DateTime.Now.AddHours(24),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return Task.FromResult(token);
        }

        public Task<(string userName ,string systemCode)> ValidateRegisterToken(string token)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationJWT.RegisterUserKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = ConfigurationJWT.Issuer,
                ValidAudience = ConfigurationJWT.Aduince,
                IssuerSigningKey = authSigningKey
            };

            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                ClaimsPrincipal claims = principal as ClaimsPrincipal;

                string name = claims.FindFirst(ClaimTypes.Name).Value;
                string systemCode = claims.FindFirst("systemCode").Value;
                return Task.FromResult((name,systemCode));
            }
            catch
            {
                return Task.FromResult((string.Empty,string.Empty));
            }
        }
    }
}
