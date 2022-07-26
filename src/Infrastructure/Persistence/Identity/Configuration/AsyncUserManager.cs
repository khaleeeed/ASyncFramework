using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.Identity.Configuration
{
    public class AsyncUserManager:UserManager<AsyncUser>
    {
        private readonly IActiveDirectoryService _ActiveDirectoryService;
        public AsyncUserManager(IActiveDirectoryService activeDirectoryService,IUserStore<AsyncUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AsyncUser> passwordHasher, IEnumerable<IUserValidator<AsyncUser>> userValidators, IEnumerable<IPasswordValidator<AsyncUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AsyncUser>> logger)
            :base(store,optionsAccessor,passwordHasher,userValidators,passwordValidators,keyNormalizer,errors,services,logger)
        {
            _ActiveDirectoryService = activeDirectoryService;
        }

        public override async Task<AsyncUser> FindByNameAsync(string userName)
        {
            var response = await _ActiveDirectoryService.GetUserByUserName(userName);
            if (response.IsSuccessful)
                return response.Data;

            return null;
        }

        public override async Task<bool> CheckPasswordAsync(AsyncUser user, string password)
        {
            var response = await _ActiveDirectoryService.Login(user.UserName,password);
            if (response.IsSuccessful)
                return true;

            return false;
        }
    }
}