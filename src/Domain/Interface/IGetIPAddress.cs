using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Interface
{
    public interface IGetIPAddress
    {
        string LocalIpAddress { get; set; }
        string RemoteIpAddress { get; set; }
    }
}
