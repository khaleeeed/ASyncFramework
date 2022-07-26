using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        public IList<UserEntity> Users { get; set; }

    }
}
