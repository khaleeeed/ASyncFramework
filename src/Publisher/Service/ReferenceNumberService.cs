using ASyncFramework.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher.Service
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