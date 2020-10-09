using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IGenericFaiulerRepository<T>: IGenericRepository<T>
    {      
        Task<bool> UpdateFaulierProcessing(string referenceNumber);
        Task UpdateFaulier(string referenceNumber, bool isSuccessfully);
        Task<(IEnumerable<T> doc, long total)> GetAllFaulierDocument(int from);
        Task<(IEnumerable<T> doc, long total)> GetAllFaulierDocumentBySystemCode(int from, string systemCode);
        Task<bool> UpdateFaulierProcessing(List<string> referenceNumber);
        Task<bool> ReverseFaulierProcessing(string referenceNumber);
    }
}