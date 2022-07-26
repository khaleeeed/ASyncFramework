using ASyncFramework.Domain.Common;
using System;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface ITokenRepository
    {
        Task<(string token, DateTime expireDate)> GenerateToken(AsyncUser user);
        Task<(string token, DateTime expireDate)> GenerateRefreshToken(string userName);
        Task<string> ValidateRefreshToken(string token);
        Task<string> GenerateRegisterToken(string userName, string systemCode);
        Task<(string userName, string systemCode)> ValidateRegisterToken(string token);
    }
}