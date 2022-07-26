using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ASyncFramework.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UI.ConsumeApi;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICallService _CallService;

        public HomeController(ICallService callService)
        {
            _CallService = callService;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            //Request.Cookies.TryGetValue(".AspNetCore.ClientCookie", out string  cookie);
            //var token = await HttpContext.GetTokenAsync("access_token");
            //var RefreshToken = await HttpContext.GetTokenAsync("refresh_token");
            //var asdfws = await HttpContext.GetTokenAsync("token_type");
            //var user = Request.HttpContext.User;
            //var claims = user.Claims;
            //var systemName = user.FindFirst("SystemCode").Value;
            //foreach (var item in claims)
            //{
            //    var items = item;
            //}
            //user.IsInRole("ADMIN");

            Request.Cookies.TryGetValue("ASyncClientCookie", out string cookie);
            var opt = Request.HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            string token = string.Empty;
            // Decrypt if found
            if (!string.IsNullOrEmpty(cookie))
            {
                var dataProtector = opt.CurrentValue.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", "ASyncClientCookie", "v2");

                var ticketDataFormat = new TicketDataFormat(dataProtector);
                var ticket = ticketDataFormat.Unprotect(cookie);
                var tokenas = ticket.Properties.GetTokens();
                token = tokenas.FirstOrDefault(x => x.Name == "access_token")?.Value;
            }

            var user = Request.HttpContext.User.Identity.Name;

            return Json(new { token, user });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Details(int queryType, string systemCode)
        {
            return View((queryType, systemCode ?? "null"));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDetailsJson(int start, int queryType, string systemCode)
        {
            string url = string.Empty;
            var token = await HttpContext.GetTokenAsync("access_token");
            if (User.IsInRole("SYSTEM") || (systemCode != null && systemCode != "null"))
            {
                systemCode = systemCode == "null" ? Request.HttpContext.User.FindFirst("SystemCode").Value : systemCode;

                switch (queryType)
                {
                    case 1:
                        url = $"api/Report/notificationDetails/system?systemCode={systemCode}&";
                        break;
                    case 2:
                        url = $"api/Report/inProcessDetails/system?systemCode={systemCode}&";
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }

            }
            else
            {

                switch (queryType)
                {
                    case 1:
                        url = $"api/Report/notificationDetails?";
                        break;
                    case 2:
                        url = $"api/Report/inProcessDetails?";
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }

            }
            var result = await _CallService.CallGet<string>($"{url}From={start}&FromDate={DateTime.Now.AddMonths(-3)}&ToDate={DateTime.Now}", token);

            return Content(result, "application/json");
        }

        [HttpGet]
        public IActionResult LutMessageStatus()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (MessageLifeCycle messageLifeCycle in Enum.GetValues(typeof(MessageLifeCycle)))
            {
                dic.Add((int)messageLifeCycle, messageLifeCycle.ToString());
            }

            return View(dic);
        }

    }
}