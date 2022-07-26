using ASyncFramework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface IServiceRepository
    {
        Task<bool> Add(ServiceEntity entity);

        Task<IEnumerable<ServiceEntity>> GetAll(string systemCode);

        bool CheckServiceActive(int serviceCode);

    }
}
