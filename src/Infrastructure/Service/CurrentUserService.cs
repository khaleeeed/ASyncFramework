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
            _ = httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("SystemCode", out Microsoft.Extensions.Primitives.StringValues userSystem);
            _ = httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("ServiceCode", out Microsoft.Extensions.Primitives.StringValues userService);

            SystemCode = userSystem;
            int.TryParse(userService,out int ServiceCode);
            this.ServiceCode = ServiceCode;
        }

        public string SystemCode { get; private set; }
        public int ServiceCode { get;  set; }
    }
}
