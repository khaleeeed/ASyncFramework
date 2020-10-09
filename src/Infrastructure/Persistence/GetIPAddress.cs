using ASyncFramework.Domain.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence
{
    public class GetIPAddress: IGetIPAddress
    {
        public GetIPAddress(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                var connections = httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>();
                LocalIpAddress = $"{connections.LocalIpAddress}:{connections.LocalPort}";
                RemoteIpAddress = $"{connections.RemoteIpAddress}:{connections.LocalPort}";
            }
            else
            {
                var addlist = Dns.GetHostEntry(Dns.GetHostName());
                LocalIpAddress= addlist.AddressList[0].ToString();
            }

        }

        public string LocalIpAddress { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}
