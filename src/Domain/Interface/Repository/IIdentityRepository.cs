using ASyncFramework.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IIdentityRepository
    {
        Task<bool> CheckPasswordAsync(string userName, string password);
        Task<AsyncUser> FindByNameAsync(string userName);
        Task<IList<string>> GetRolesAsync(AsyncUser user);
        Task AddRole(AsyncUser user, string role);
        Task RemoveRole(AsyncUser user, string role);
    }
}