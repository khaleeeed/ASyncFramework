using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IGenericFaiulerRepository<T>: IGenericRepository<T>
    {
        Task<bool> UpdateFaulierProcessing(string referenceNumber, byte[] timeStampCheck);
        Task UpdateFaulier(string referenceNumber, bool isSuccessfully);
        Task<(IEnumerable<T> doc, long total)> GetAllFaulierDocument(int from);
        Task<(IEnumerable<T> doc, long total)> GetAllFaulierDocumentBySystemCode(int from, string systemCode);
        Task<string> UpdateFaulierProcessing(List<string> referenceNumber, List<byte[]> timeStampChecks);
        Task<bool> ReverseFaulierProcessing(string referenceNumber);
    }
}