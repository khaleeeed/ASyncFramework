using ASyncFramework.Domain.Interface;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace ASyncFramework.Domain.Service
{
    public class AllHeadersPerRequest : IAllHeadersPerRequest
    {
        public AllHeadersPerRequest(IHttpContextAccessor httpContextAccessor)
        {
            var tryGetValue = httpContextAccessor.HttpContext?.Request?.Headers?.ToDictionary(k => k.Key, k => k.Value.ToString());
            Headrs = tryGetValue;
        }

        public Dictionary<string, string> Headrs { get; private set; }
    }
}