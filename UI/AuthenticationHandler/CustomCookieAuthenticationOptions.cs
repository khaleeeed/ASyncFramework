using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace UI.AuthenticationHandler
{
    public class CustomCookieAuthenticationOptions : AuthenticationSchemeOptions
    {
        public CustomCookieAuthenticationOptions()
        {
        }
        
    }

    internal class CustomCookieAuthenticationHandler : AuthenticationHandler<CustomCookieAuthenticationOptions>
    {

        public CustomCookieAuthenticationHandler(
            IOptionsMonitor<CustomCookieAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {


            Request.Cookies.TryGetValue("ASyncClientCookie", out string cookie);

            var opt = Request.HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();

            // Decrypt if found
            if (!string.IsNullOrEmpty(cookie))
            {
                var dataProtector = opt.CurrentValue.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", "ASyncClientCookie", "v2");

                var ticketDataFormat = new TicketDataFormat(dataProtector);
                var ticket = ticketDataFormat.Unprotect(cookie);
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("test");
        }
    }
}
