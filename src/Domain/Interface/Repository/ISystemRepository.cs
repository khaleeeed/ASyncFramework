using ASyncFramework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface ISystemRepository
    {
        Task<bool> Add(SystemEntity entity);
        (bool isActive, bool hasCustomQueue) CheckSystemActive(string systemCode);
        Task<IEnumerable<SystemEntity>> GetAll();
        SystemEntity GetSystem(string systemCode);
        Task<bool> Update(int systemCode, string enSystemName, string arSystemName, bool isActive, bool hasCustomQueue);
    }
}
