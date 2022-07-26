using ASyncFramework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface IPushFailuerRepository 
    {
        Task<bool> Add(PushFailuerEntity entity);
        Task<IEnumerable<PushFailuerEntity>> GetAll();
        Task UpdateIsActive(long id);
    }
}
