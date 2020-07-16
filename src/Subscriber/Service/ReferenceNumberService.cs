using ASyncFramework.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subscriber.Service
{
    public class ReferenceNumberService : IReferenceNumberService
    {
        public ReferenceNumberService()
        {
            ReferenceNumber = new Guid().ToString();
        }

        public string ReferenceNumber { get; }
    }
}