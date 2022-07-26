using ASyncFramework.Domain.Interface;
using System;

namespace ASyncFramework.Domain.Service
{
    public class ReferenceNumberService : IReferenceNumberService
    {

        private string _ReferenceNumber;
        public string ReferenceNumber 
        {
            get 
            {
                return _ReferenceNumber ?? Guid.NewGuid().ToString();
            } 
            set 
            {
                _ReferenceNumber = value;
            } 
        }
    }
}