using Microsoft.AspNetCore.Mvc;

namespace QuartzManager.Controllers
{
    [Route("")]
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return Redirect(@"~/Quartzmin");
        }

        [Route("keepAlive")]
        [HttpGet]
        public IActionResult KeepAlive()
        {
            return Ok("alive");
        }
    }
}