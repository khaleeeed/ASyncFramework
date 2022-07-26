using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.Identity.Repo
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<AsyncUser> _UserManager;

        public IdentityRepository(UserManager<AsyncUser> userManager)
        {
            _UserManager = userManager;
        }

        public Task<AsyncUser> FindByNameAsync(string userName)
        {
            return _UserManager.FindByNameAsync(userName);
        }

        public Task<bool> CheckPasswordAsync(string userName, string password)
        {
            return _UserManager.CheckPasswordAsync(new AsyncUser { UserName = userName }, password);
        }

        public Task<IList<string>> GetRolesAsync(AsyncUser user)
        {
            return _UserManager.GetRolesAsync(user);
        }

        public Task AddRole(AsyncUser user,string role)
        {
            return _UserManager.AddToRoleAsync(user, role);

        }       

        public Task RemoveRole(AsyncUser user, string role)
        {
            return _UserManager.RemoveFromRoleAsync(user, role);
        }
    }
}
