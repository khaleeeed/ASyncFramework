using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public int SystemId { get; set; }
        public bool IsActive { get; set; }
        public SystemEntity System { get; set; }
        public RoleEntity Role { get; set; }
    }
}