using ASyncFramework.Domain.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface ILUTNotificationRepository:IGenericRepository<NotificationDocument>
    {
        Task<(IEnumerable<NotificationDocument> doc, long total)> GetDocumentBySystemCode(int from, string systemCode);
        Task<bool> AddNotification(string systemCode, string samplePayload, NotificationFields notification);
        Task<bool> UpdateSystemNotification(string systemName, string systemArName, string systemCode);
        Task<bool> UpdateNotification(string samplePayload, NotificationFields notification);
        Task<int> GetNewSystemCode();
    }
}