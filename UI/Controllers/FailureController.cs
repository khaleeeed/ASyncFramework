using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.ConsumeApi;
using UI.Models;

namespace UI.Controllers
{
    [Authorize]
    public class FailureController : Controller
    {
        private readonly ICallService _CallService;

        public FailureController(ICallService callService)
        {
            _CallService = callService;
        }
       
        [HttpGet]
        public IActionResult Targets()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTargets(int start)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Failure/target/messages?From={start}", token);

            return Content(result, "application/json");

        }
        [HttpPost]
        public async Task<IActionResult> SendRetryRequest(RetryFailureCommand command)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var body= Newtonsoft.Json.JsonConvert.SerializeObject(command);
            var result = await _CallService.CallPost<string>($"api/failures/target",body, token);

            return Content(result, "application/json");
        }

        [HttpGet]
        public IActionResult CallBacks()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetCallBack(int start)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Failure/callback/messages?From={start}", token);

            return Content(result, "application/json");

        }
        [HttpPost]
        public async Task<IActionResult> SendRetryCallBack(RetryFailureCommand command)
        {

            var token = await HttpContext.GetTokenAsync("access_token");

            var body = Newtonsoft.Json.JsonConvert.SerializeObject(command);
            var result = await _CallService.CallPost<string>($"api/failures/callback", body, token);

            return Content(result, "application/json");
        }

        [HttpGet]
        public IActionResult TargetsSearchBySystem()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTargetsSearchBySystem(int start, string systemId)
        {
            if (systemId is null)
            {
                systemId = User.FindFirst(x => x.Type == "SystemCode").Value;
            }
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/failure/target/messages/systemcode/{systemId}?From={start}", token);

            return Content(result, "application/json");

        }

        [HttpGet]
        public IActionResult CallBackSearchBySystem()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetCallBackSearchBySystem(int start,string systemId)
        {
            if (systemId is null)
            {
                systemId = User.FindFirst(x => x.Type == "SystemCode").Value;
            }
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/failure/callback/messages/systemcode/{systemId}?From={start}", token);

            return Content(result, "application/json");

        }

        [HttpGet]
        public IActionResult TargetsForSystem()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult CallBackForSystem()
        {
            return View();
        }
    }

}
