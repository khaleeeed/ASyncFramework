using ASyncFramework.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subscriber.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var tryGetValue = httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("SystemUser",out Microsoft.Extensions.Primitives.StringValues user);
            SystemUser = user;
        }

        public string SystemUser { get; }
    }
}
