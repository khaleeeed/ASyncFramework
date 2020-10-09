using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Application.Common.Models
{
    public class UnregisterHeader
    {
        public static List<string> UnregisteredHeaders = new List<string>
        {
            "Content-Type",
            "Content-Length",
            "Host"
        };
    }
}
