using ASyncFramework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface IDisposeRepository 
    {
        Task<bool> Add(DisposeEntity doc);
        Task<DisposeEntity> FindDocument(string referenceNumber);
    }
}