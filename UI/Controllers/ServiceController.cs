using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.ConsumeApi;

namespace UI.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly ICallService _CallService;

        public ServiceController(ICallService callService)
        {
            _CallService = callService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllService(string systemCode)
        {
            if (systemCode == null || systemCode == "null")
            {
                systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;
            }

            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Service?SystemCode={systemCode}", token);

            return Content(result, "application/json");
        }

        public IActionResult AddService()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddService(int serviceCode, string systemCode, string arDiscription, string enDiscription)
        {
            if (systemCode == null || systemCode == "null")
            {
                systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;
            }
            var token = await HttpContext.GetTokenAsync("access_token");

            var jsonCommand = Newtonsoft.Json.JsonConvert.SerializeObject(new { ServiceCode=serviceCode,SystemCode = systemCode,ArDiscription=arDiscription,EnDiscription=enDiscription});

            var result = await _CallService.CallPost<Result>("api/Service", jsonCommand, token);

            return Json(result);
        }
    }
}
