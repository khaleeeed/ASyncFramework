using System.Threading.Tasks;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using UI.ConsumeApi;

namespace UI.Controllers
{
    public class SystemController : Controller
    {
        private readonly ICallService _CallService;

        public SystemController(ICallService callService)
        {
            _CallService = callService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSystem()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>("api/GetAllSystem", token);

            return Content(result, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSystem(string systemCode)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<SystemEntity>($"api/system?systemCode={systemCode}", token);

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSystem(SystemEntity queue)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var jsonQueue = Newtonsoft.Json.JsonConvert.SerializeObject(queue);
            var result = await _CallService.CallPost<Result>($"api/updateSystem", jsonQueue, token);

            return Json(result);
        }

        [HttpGet]
        public IActionResult AddSystem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSystem(string arSystemName , string enSystemName, bool hasCustomQueue, bool isActive, int systemCode)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var jsonQueue = Newtonsoft.Json.JsonConvert.SerializeObject(new { SystemCode=systemCode, EnSystemName= enSystemName, arSystemName= arSystemName, HasCustomQueue=hasCustomQueue , IsActive=isActive  });
            var result = await _CallService.CallPost<Result>($"api/System", jsonQueue, token);

            return Json(result);
        }
    }
}
