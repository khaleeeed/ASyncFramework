using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.ConsumeApi;

namespace UI.Controllers
{
   
    public class ReportController : Controller
    {
        private readonly ICallService _CallService;

        public ReportController(ICallService callService)
        {
            _CallService = callService;
        }
        public async Task<IActionResult> NotificationCount( DateTime from , DateTime to)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/notificationCount?from={from}&to={to}", token);

            return Content(result, "application/json");
        }
        public async Task<IActionResult> NotificationCountBySystemQuery(DateTime from, DateTime to, string systemCode)
        {
            if (systemCode==null || systemCode =="null")
            {
                systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;
            }
            
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/notificationCount/system?from={from}&to={to}&systemCode={systemCode}", token);

            return Content(result, "application/json");
        }


        public async Task<IActionResult> FinshNotificationCountBySystemQuery(DateTime from, DateTime to ,string systemCode)
        {
            if (systemCode == null || systemCode == "null")
            {
                systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;
            }

            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/finshNotificationCount/system?from={from}&to={to}&systemCode={systemCode}", token);

            return Content(result, "application/json");
        }
        public async Task<IActionResult> FinshNotificationCount(DateTime from, DateTime to)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/finshNotificationCount?from={from}&to={to}", token);

            return Content(result, "application/json");
        }

    
        public async Task<IActionResult> TargetFailureCountQuery(DateTime from, DateTime to)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/targetFailure?from={from}&to={to}", token);

            return Content(result, "application/json");
        }      
        public async Task<IActionResult> TargetFailureCountBySystemQuery(DateTime from, DateTime to,string systemCode)
        {
            if (systemCode == null || systemCode == "null")
            {
                systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;
            }
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/targetFailure/system?from={from}&to={to}&systemCode={systemCode}", token);

            return Content(result, "application/json");
        }

     
        public async Task<IActionResult> CallBackFailureCountQuery(DateTime from, DateTime to)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/callbackFailure?from={from}&to={to}", token);

            return Content(result, "application/json");
        }     
        public async Task<IActionResult> CallBackFailureCountBySystemQuery(DateTime from, DateTime to, string systemCode)
        {
            if (systemCode == null || systemCode == "null")
            {
                systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;
            }

            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/callbackFailure/system?from={from}&to={to}&systemCode={systemCode}", token);

            return Content(result, "application/json");
        }

      
        public async Task<IActionResult> MessageInProcessCountQuery(DateTime from, DateTime to)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/inProcess?from={from}&to={to}", token);

            return Content(result, "application/json");
        }    
        public async Task<IActionResult> MessageInProcessCountBySystemQuery(DateTime from, DateTime to, string systemCode)
        {
            if (systemCode == null || systemCode == "null")
            {
                systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;
            }

            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/report/inProcess/system?from={from}&to={to}&systemCode={systemCode}", token);

            return Content(result, "application/json");
        }
    }
}
