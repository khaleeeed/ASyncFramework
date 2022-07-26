using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Common
{
    public class AsyncUser
    {
        public AsyncUser()
        {

        }
        private string _Roles;
        public string Roles { get { return _Roles; } set { var array = value?.Split("-"); if (array?.Length == 2) { _Roles = array[0]; System = array[1]; } } }
        public string System { get; private set; }
        public string Id { get; set; }
        public string UserName { get; set; }        
    }
}