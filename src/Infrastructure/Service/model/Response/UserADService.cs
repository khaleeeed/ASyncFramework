using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ASyncFramework.Infrastructure.Service.model.Response
{
    public class UserADService
    {
        public Data[] data { get; set; }

    }
    public class Data
    {
        public User user{ get; set; }
    }
    public class User
    {
        public string EmployeeID { get; set; }
        public string UserName { get; set; }
        public string Mail { get; set; }
    }
}