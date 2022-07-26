using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface ICallBackFailureRepository : IGenericFaiulerRepository<CallBackFailuerEntity>
    {
        Task<long> CountForAdmin(DateTime from, DateTime to);
        Task<long> CountForSystem(DateTime from, DateTime to, string systemCode);
    }
}
