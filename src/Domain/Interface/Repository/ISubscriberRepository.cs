using ASyncFramework.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface ISubscriberRepository
    {
        Task<bool> Add(bool isRunning);
        Task<IEnumerable<SubscriberEntity>> GetAll();
        Task<bool> UpdateIsRunning(bool isRunning);
        Task<bool> UpdateTimeOfTakeConfiguration(DateTime timeOfTakeConfiguration);
        Task<bool> AddOrUpdate(bool isRunning);
        Task<bool> UpdateIsRunning(bool isRunning, byte[] timeStampCheck);
        Task<IEnumerable<string>> GetAllUrl();
    }
}