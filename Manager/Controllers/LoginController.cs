using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Application.Manager.Account.Command;
using ASyncFramework.Application.Manager.Account.Command.RegisterUserCommandHandler;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure.Persistence.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Manager.Controllers
{
    public class LoginController : Controller
    {
        private IMediator _mediator;
        private IMediator Mediator => _mediator ??= (IMediator)HttpContext.RequestServices.GetService(typeof(IMediator));

        [HttpGet]
        public IActionResult SignIn(
        string response_type, 
        string client_id,
        string redirect_uri,
        string scope, 
        string state) 
        {
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri);
            query.Add("state", state);

            return View(model: query.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(
            string userName,
            string password,
            string redirectUri,
            string state)
        {
            var result = await Mediator.Send(new CreateTokenCommand { UserName = userName, Password = password });

            if (!result.IsSuccessful)
            {
                var redirectQuery = new QueryBuilder();
                redirectQuery.Add("redirectUri", redirectUri);
                redirectQuery.Add("state", state);
                ViewBag.Error = "error";
                return View(model: redirectQuery.ToString());
            }

            var query = new QueryBuilder();
            query.Add("code", result.Data.Token + "$$" + result.Data.RefreshToken +"$$"+ result.Data.ExpireTime);
            query.Add("state", state);

            return Redirect($"{redirectUri}{query}");
        }

        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id)
        {
            var split = code.Split("$$");

            var responseObject = new
            {
                access_token=split[0],
                refresh_token=split[1],
                expires_at = split[2]
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string userName,string systemCode,string systemName)
        {
            var command = new RegisterUserCommand() { UserName=userName,SystemName=systemName,SystemCode=systemCode};
            var result = Mediator.Send(command);
            return View();
        }

    }
}