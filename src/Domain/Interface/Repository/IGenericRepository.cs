using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IGenericRepository<T>
    {
        Task<T> FindDocument(string referenceNumber);
        Task<bool> Add(T doc);
        Task<(IEnumerable<T> doc, long total)> GetAllDocument(int from);
    }
}
