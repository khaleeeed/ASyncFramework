using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class ServiceEntity
    {
        public long Id { get; set; }
        public int ServiceCode { get; set; }
        public string SystemCode { get; set; }
        public string ArDiscription { get; set; }
        public string EnDiscription { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }

    }
}
