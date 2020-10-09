using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Publisher.Controllers
{
    [Route("")]
    [ApiController]
    [OpenApiIgnore]
    public class DefaultController : ControllerBase
    {
        public IActionResult Get()
        {
            return Redirect(@"~/swagger");
        }
    }

}
