using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UI.ConsumeApi;

namespace UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ICallService _CallService;

        public AccountController(ICallService callService)
        {
            _CallService = callService;
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync("ClientCookie");
            return RedirectToAction("index","home");
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(string userName, string systemCode, int role)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            Result result = new Result(false, null);
            var model = JsonConvert.SerializeObject(new { UserName = userName, SystemCode = systemCode });
           
            if (role == 0)
                result = await _CallService.CallPost<Result>("api/account/admin", model, token);

            else if (role == 1)
                result = await _CallService.CallPost<Result>("api/account/userSystem", model, token);


            return Json(result);
        }

        [HttpGet]
        public IActionResult RemoveUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveUser(string userName)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            Result result = new Result(false, null);
            var model = JsonConvert.SerializeObject(new { UserName = userName });

            result = await _CallService.CallPost<Result>("api/account/remove", model, token);

            return Json(result);
        }

        [HttpGet("api/systems")]
        public async Task<IActionResult> GetAllSystems()
        {
            var systems = await _CallService.CallGet<List<MOJSystemResponse>>("api/systems");
            return Json(systems);
        }        
    }
}