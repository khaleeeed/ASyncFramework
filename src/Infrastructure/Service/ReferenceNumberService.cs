using ASyncFramework.Domain.Interface;
using System;

namespace ASyncFramework.Domain.Service
{
    public class ReferenceNumberService : IReferenceNumberService
    {
        public ReferenceNumberService()
        {
            ReferenceNumber = Guid.NewGuid().ToString();
        }

        public string ReferenceNumber { get; private set; }
    }
}