using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UI.ConsumeApi;

namespace UI.Controllers
{
    [Authorize]
    public class MessageEnquiryController : Controller
    {
        private readonly ICallService _CallService;

        public MessageEnquiryController(ICallService callService)
        {
            _CallService = callService;
        }

        [HttpGet]
        public IActionResult StatusMessage()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusMessage(string referenceNumber, int start)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Message/status?ReferenceNumber={referenceNumber}&From={start}", token);

            return Content(result, "application/json");

        }

        [HttpGet]
        public IActionResult TargetResponse()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTargetResponse(string referenceNumber, int start)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Message/targetResponse?ReferenceNumber={referenceNumber}&From={start}", token);

            return Content(result, "application/json");

        }

        [HttpGet]
        public IActionResult CallBackResponse()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCallBackResponse(string referenceNumber, int start)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Message/callBackResponse?ReferenceNumber={referenceNumber}&From={start}", token);

            return Content(result, "application/json");

        }

        [HttpGet]
        public IActionResult CallBackUrlData()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCallBackUrlData(string url, int start)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Message/callBackUrl?CallBackUrl={url}&From={start}", token);

            return Content(result, "application/json");
        }

        [HttpGet]
        public IActionResult ContentBodyForAdmin()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetContentBodyForAdmin(string fieldValue, string fieldName, int start)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Message/ContentBodyForAdmin?fieldValue={fieldValue}&fieldName={fieldName}&From={start}", token);

            return Content(result, "application/json");
        }



        [HttpGet]
        public IActionResult ContentBodyForSystem()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetContentBodyForSystem(string fieldValue, string fieldName, int start)
        {
            var systemCode = User.FindFirst(x => x.Type == "SystemCode").Value;

            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _CallService.CallGet<string>($"api/Message/ContentBodyForSystem?fieldValue={fieldValue}&fieldName={fieldName}&SystemCode={systemCode}&From={start}", token);

            return Content(result, "application/json");
        }
    }
}