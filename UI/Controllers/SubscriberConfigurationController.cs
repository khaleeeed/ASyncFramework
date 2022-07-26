using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.ConsumeApi;
using UI.Models;

namespace UI.Controllers
{
    [Authorize]
    public class SubscriberConfigurationController : Controller
    {
        private readonly ICallService _CallService;

        public SubscriberConfigurationController(ICallService callService)
        {
            _CallService = callService;
        }
        [HttpGet]
        public IActionResult Subscribers()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubscribers()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>("api/Subscriber", token);

            return Content(result, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> Stop(UpdateSubscriberCommand command)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            string url;
            object obj;
            if (command.Url.Length == 1)
            {
                url = "api/Subscriber/stop";
                obj = new { Url = command.Url.FirstOrDefault(), TimeStampCheck = command.TimeStampCheck.FirstOrDefault() };
            }
            else
            {
                url = "api/subscribers/stop";
                obj = new { Urls = command.Url, TimeStampChecks = command.TimeStampCheck };
            }


            var jsonCommand = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var result = await _CallService.CallPost<Result>(url, jsonCommand, token);
            return Json(result);

        }

        [HttpPost]
        public async Task<IActionResult> Start(UpdateSubscriberCommand command)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            string url;
            object obj;
            if (command.Url.Length == 1)
            {
                url = "api/Subscriber/start";
                obj = new { Url = command.Url.FirstOrDefault(), TimeStampCheck = command.TimeStampCheck.FirstOrDefault() };
            }
            else
            {
                url = "api/subscribers/start";
                obj = new { Urls = command.Url, TimeStampChecks = command.TimeStampCheck };
            }
            var jsonCommand = Newtonsoft.Json.JsonConvert.SerializeObject(obj);


            var result = await _CallService.CallPost<Result>(url, jsonCommand, token);
            return Json(result);

        }

        [HttpPost]
        public async Task<IActionResult> ForceStop(UpdateSubscriberCommand command)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            string url;
            object obj;
            if (command.Url.Length == 1)
            {
                url = "api/Subscriber/forceStop";
                obj = new { Url = command.Url.FirstOrDefault(), TimeStampCheck = command.TimeStampCheck.FirstOrDefault() };
            }
            else
            {
                url = "api/subscribers/forceStop";
                obj = new { Urls = command.Url, TimeStampChecks = command.TimeStampCheck };
            }
            var jsonCommand = Newtonsoft.Json.JsonConvert.SerializeObject(obj);


            var result = await _CallService.CallPost<Result>(url, jsonCommand, token);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshQueueConfiguration(string url)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            object obj=null;

            if (url != null)
            {
                obj = new { url };
                url = "api/QueueConfiguration/refresh";
            }
            else            
                url = "api/QueueConfiguration/refreshAllSubscriber";
            
            
            var jsonCommand = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var result = await _CallService.CallPost<Result>(url, jsonCommand, token);
            return Json(result);
        }
    }
}