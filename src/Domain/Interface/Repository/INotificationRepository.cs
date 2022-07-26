using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface INotificationRepository
    {

        (bool IsSendBefore, string ReferenceNumber) CheckIfRequestSendBefore(string hash, string id);
        long Add(NotificationEntity entity);
        Task<long> CountForAdmin(DateTime from, DateTime to);
        Task<long> CountForSystem(DateTime from, DateTime to, string systemCode);
        Task<long> CountInProcessForSystem(DateTime from, DateTime to, string systemCode);
        Task<long> CountInProcessForAdmin(DateTime from, DateTime to);
        void UpdateStatusId(string referenceNumber, MessageLifeCycle messageLifeCycle);
        Task<long> CountFinshForSystem(DateTime from, DateTime to, string systemCode);
        Task<long> CountFinshForAdmin(DateTime from, DateTime to);
        Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsForAdmin(int from, DateTime fromDate, DateTime toDate);
        Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsForSystem(int from, DateTime fromDate, DateTime toDate, string systemCode);
        Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsInProcessForAdmin(int from, DateTime fromDate, DateTime toDate);
        Task<(IEnumerable<NotificationEntity> doc, long total)> DetailsInProcessForSystem(int from, DateTime fromDate, DateTime toDate, string systemCode);
    }

}       
