using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.ConsumeApi;

namespace UI.Controllers
{
    [Authorize]
    public class QueueConfigurationController : Controller
    {
        private readonly ICallService _CallService;

        public QueueConfigurationController(ICallService callService)
        {
            _CallService = callService;
        }

        [HttpGet]
        public IActionResult QueuesConfigurations()
        {
            return View();
        }

        [HttpGet]   
        public async Task<IActionResult> GetAllQueueConfiguration()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>("api/QueuesConfigurations", token);

            return Content(result, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateQueuesConfigurations(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<QueueConfigurations>($"api/QueueConfiguration?id={id}", token);

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQueuesConfigurations(QueueConfigurations queue)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var jsonQueue= Newtonsoft.Json.JsonConvert.SerializeObject(queue);
            var result = await _CallService.CallPost<Result>($"api/updateQueuesConfigurations", jsonQueue, token);

            return Json(result);
        }

        [HttpGet]
        public IActionResult AddQueuesConfigurations()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddQueuesConfigurations(QueueConfigurations queue)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var jsonQueue = Newtonsoft.Json.JsonConvert.SerializeObject(new { data = queue });
            var result = await _CallService.CallPost<Result>($"api/QueueConfiguration", jsonQueue, token);

            return Json(result);
        }
    }
}