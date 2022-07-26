using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.Identity.Configuration
{
    public class AsyncUserStoreService : IUserStore<AsyncUser>, IUserRoleStore<AsyncUser>
    {
        private readonly IActiveDirectoryService _ActiveDirectoryService;
        private readonly IUserRepository _UserRepository;

        public AsyncUserStoreService(IActiveDirectoryService activeDirectoryService,IUserRepository userRepository)
        {
            _ActiveDirectoryService = activeDirectoryService;
            _UserRepository = userRepository;
        }

        public Task<IList<string>> GetRolesAsync(AsyncUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<string>) new List<string> { user.Roles });
        }
        public Task<bool> IsInRoleAsync(AsyncUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult($"{user.Roles}-{user.System}"==roleName);
        }
        public async Task<AsyncUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var response = await _ActiveDirectoryService.GetUserByUserName(normalizedUserName);
            if (response.IsSuccessful)
                return response.Data;

            return null;
        }
        public Task AddToRoleAsync(AsyncUser user, string roleName, CancellationToken cancellationToken)
        {
            var array = user.UserName.Split('@');
            var username = user.UserName;
            if (array != null)
                username = array[0];
            user.Roles = roleName;
            return _UserRepository.Add(username, user.System,user.Roles);            
        }
        public Task RemoveFromRoleAsync(AsyncUser user, string roleName, CancellationToken cancellationToken)
        {
            return _UserRepository.RemoveUser(user.UserName);
        }
        public Task<string> GetUserNameAsync(AsyncUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }
        public Task<string> GetNormalizedUserNameAsync(AsyncUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<IdentityResult> CreateAsync(AsyncUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(AsyncUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AsyncUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }             

        public Task<string> GetUserIdAsync(AsyncUser user, CancellationToken cancellationToken)
        {
            var array = user.UserName.Split('@');
            var username = user.UserName;
            if (array != null)
                username = array[0];

            return Task.FromResult(username);
        }

        public Task<IList<AsyncUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }      

        public Task SetNormalizedUserNameAsync(AsyncUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AsyncUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(AsyncUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}