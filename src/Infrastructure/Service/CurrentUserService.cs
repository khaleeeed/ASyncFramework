using ASyncFramework.Domain.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _ = httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("SystemCode", out Microsoft.Extensions.Primitives.StringValues user);
            SystemCode = user;
        }

        public string SystemCode { get; private set; }
    }
}
