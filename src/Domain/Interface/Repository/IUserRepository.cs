using ASyncFramework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface IUserRepository
    {
        Task<bool> Add(string userName, string systemCode, string roleName);
        UserEntity GetUser(string userName);
        Task<bool> RemoveUser(string userName);
    }
}
