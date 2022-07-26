using System;
using System.Threading.Tasks;
using ASyncFramework.Application.Manager.Account.Command;
using ASyncFramework.Application.Manager.Account.Command.AddAdminUserRoleCommandHandler;
using ASyncFramework.Application.Manager.Account.Command.AddUserSystemRoleCommandHandler;
using ASyncFramework.Application.Manager.Account.Command.ConfirmationAddUserCommandHandler;
using ASyncFramework.Application.Manager.Account.Command.CreateNewTokenFromRefreshTokenCommandHandler;
using ASyncFramework.Application.Manager.Account.Command.RegisterUserCommandHandler;
using ASyncFramework.Application.Manager.Account.Command.RemoveUserCommandHandler;
using ASyncFramework.Application.Manager.Account.Query.SystemsQueryHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ApiController
    {
        /// <summary>
        /// Generate Token    
        /// </summary>
        [HttpPost("token")]
        public async Task<IActionResult> Token(CreateTokenCommand command)
        {
            var result = await Mediator.Send(command);

            switch (result.StatusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    return Unauthorized(result);
                case System.Net.HttpStatusCode.NotFound:
                    return NotFound(result);
                default:
                    { SetTokenCookie(result.Data.RefreshToken); return Ok(result); }
            }          
        }
        /// <summary>
        /// Refresh Token if expired 
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(CreateNewTokenCommand command)
        {
            var result = await Mediator.Send(command);

            switch (result.StatusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    return Unauthorized(result);
                default:
                    { SetTokenCookie(result.Data.RefreshToken); return Ok(result); }
            }
        }

        /// <summary>
        /// Add Admin
        /// </summary>
        [HttpPost("admin")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddAdmin(AddAdminRoleCommand command)
        {
            var result =await Mediator.Send(command);
            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Add user system
        /// </summary>
        [HttpPost("userSystem")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddUserSystem(AddUserSystemRoleCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPost("remove")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RemoveUser(RemoveUserCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }
        /// <summary>
        /// Create User    
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Confirmation Add User by token    
        /// </summary>
        [HttpGet("confirmation")]
        public async Task<IActionResult> ConfirmationAddUser(string token)
        {
            var result = await Mediator.Send(new ConfirmationAddUserCommand { Token = token });

            if (result.Succeeded)
                return Ok(result);

            return UnprocessableEntity(result);
        }

        /// <summary>
        /// Get All System    
        /// </summary>
        [HttpGet("/api/systems")]
        public async Task<IActionResult> GetAllSystem()
        {
            return Ok(await Mediator.Send(new SystemsQuery()));
        }

        [NonAction]
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(7)
            };
            Response.Cookies.Append("refresh_token",token, cookieOptions);
        }   
    }
}