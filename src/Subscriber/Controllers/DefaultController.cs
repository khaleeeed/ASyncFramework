using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Subscriber.Controllers
{
    [Route("")]
    [ApiController]    
    public class DefaultController : ControllerBase
    {

        public IActionResult Get()
        {
            return Redirect("~/health");
        }

        [Route("keepAlive")]
        [HttpGet]
        public IActionResult KeepAlive()
        {
            return Ok("alive");
        }
        
    }
}