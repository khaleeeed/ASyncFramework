using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class SystemEntity
    {
        public int Id { get; set; }
        public int SystemCode { get; set; }
        public string EnSystemName { get; set; }
        public string ArSystemName { get; set; }
        public bool IsActive { get; set; }
        public bool HasCustomQueue { get; set; }

        public IList<UserEntity> Users { get; set; }
    }
}
