using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using UI.ConsumeApi;

namespace UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);

            });


            services.ConfigureApplicationCookie(options => options.Cookie.SameSite = SameSiteMode.Lax);
            services.ConfigureExternalCookie(options =>
            {
                options.Cookie.IsEssential = true;
                // Other options
                options.Cookie.SameSite = SameSiteMode.Lax;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.IsEssential = true;
                // Other options
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.Configure<CookieAuthenticationOptions>(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });


            services.AddSingleton(typeof(ICallService), typeof(CallService));

            services.AddAuthentication(config => {
                // We check the cookie to confirm that we are authenticated
                config.DefaultAuthenticateScheme = "ClientCookie";
                // When we sign in we will deal out a cookie
                config.DefaultSignInScheme = "ClientCookie";
                // use this to check if we are allowed to do something.
                config.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie", COA => {
                    COA.Cookie.SameSite = SameSiteMode.Lax;
                    COA.Cookie.IsEssential = true;
                    COA.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async context =>
                        {
                            var tokens = context.Properties.GetTokens();
                            var refreshToken = tokens.FirstOrDefault(t => t.Name == "refresh_token");
                            var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token");
                            var exp = tokens.FirstOrDefault(t => t.Name == "expires_at");
                            var expireTime = DateTime.Parse(exp.Value);

                            if (DateTime.UtcNow > expireTime)
                            {
                                if (refreshToken == null || DateTime.UtcNow > expireTime.AddHours(6))
                                {
                                    context.RejectPrincipal();
                                    return;
                                }

                                var callservice = context.HttpContext.RequestServices.GetService<ICallService>();
                                var body = JsonConvert.SerializeObject(new { refreshtoken = refreshToken.Value });
                                var tokenresponse = await callservice.CallPost<GenericServiceResponse<TokenResponse>>("api/account/refresh-token", body);
                                if (!tokenresponse.IsSuccessful)
                                {
                                    context.RejectPrincipal();
                                    return;
                                }
                                refreshToken.Value = tokenresponse.Data.RefreshToken;
                                accessToken.Value = tokenresponse.Data.Token;
                                exp.Value = tokenresponse.Data.ExpireTime.ToUniversalTime().ToString();

                                context.Properties.StoreTokens(tokens);
                                context.ShouldRenew = true;
                            }
                        }
                    };
                })
                .AddOAuth("OurServer", config => {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = new PathString("/oauth/callback");
                    config.AuthorizationEndpoint = Configuration.GetValue<string>("AuthorizeUrl");
                    config.TokenEndpoint = Configuration.GetValue<string>("TokenUrl");
                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context =>
                        {

                            try
                            {
                                var tokenHandler = new JwtSecurityTokenHandler();
                                var token = context.AccessToken;
                                var jwtToken = tokenHandler.ReadJwtToken(token);
                                var claims = jwtToken.Claims;
                                foreach (var claim in claims)
                                {
                                    context.Identity.AddClaim(new Claim(claim.Type, claim.Value));
                                }
                                var tokens = new List<AuthenticationToken>
                                {
                                    new AuthenticationToken{Name = "access_token" ,Value= context.Properties.GetTokenValue("access_token")},
                                    new AuthenticationToken{Name="refresh_token", Value= context.Properties.GetTokenValue("refresh_token")},
                                    new AuthenticationToken{Name="expires_at", Value= DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(claims.FirstOrDefault(f=>f.Type=="exp").Value)).DateTime.ToString()}
                                };
                                context.Properties.StoreTokens(tokens);
                                return Task.CompletedTask;
                            }
                            catch
                            {
                                return Task.FromResult(string.Empty);
                            }



                        }
                    };
                });


            services.AddHttpClient();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                 .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
                 .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
                OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions),
                OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions)

            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }
        public static bool DisallowsSameSiteNone(string userAgent)
        {
            // Cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking
            // stack.
            if (userAgent.Contains("CPU iPhone OS 12") ||
                userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            // Cover Mac OS X based browsers that use the Mac OS networking stack. 
            // This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }

            // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions, 
            // but pre-Chromium Edge does not require SameSite=None.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6") || userAgent.Contains("Chrome/7") || userAgent.Contains("Chrome/8") || userAgent.Contains("Chrome/9"))
            {
                return true;
            }

            return false;
        }
    }
}