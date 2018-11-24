using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Content.Mvc.Infrastructure;
using Content.Mvc.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;

namespace Content.Mvc
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
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddOptions();
            services.Configure<AppSettings>(Configuration);

            services.AddHttpClientServices(Configuration);
            services.AddCustomAuthentication(Configuration);
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider =
                        (type, factory) => factory.Create(typeof(SharedResource));
                }); ;

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("zh"),
                };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("zh"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseAuthentication();

            //app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");

            // Add Authentication services          

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie("Cookies", options =>
            {
                //options.Cookie.HttpOnly = false;
                //options.Cookie.SameSite = SameSiteMode.None;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.None;

                options.Events = new CookieAuthenticationEvents
                {
                    // this event is fired everytime the cookie has been validated by the cookie middleware,
                    // so basically during every authenticated request
                    // the decryption of the cookie has already happened so we have access to the user claims
                    // and cookie properties - expiration, etc..
                    OnValidatePrincipal = async x =>
                    {
                        // since our cookie lifetime is based on the access token one,
                        // check if we're more than halfway of the cookie lifetime
                        var now = DateTimeOffset.UtcNow;
                        var timeElapsed = now.Subtract(x.Properties.IssuedUtc.Value);
                        var timeRemaining = x.Properties.ExpiresUtc.Value.Subtract(now);

                        if (timeElapsed > timeRemaining.Subtract(TimeSpan.FromMinutes(1)))
                        {
                            var identity = (ClaimsIdentity)x.Principal.Identity;
                            var accessTokenClaim = identity.FindFirst("access_token");
                            var refreshTokenClaim = identity.FindFirst("refresh_token");

                            // if we have to refresh, grab the refresh token from the claims, and request
                            // new access token and refresh token
                            var refreshToken = refreshTokenClaim.Value;
                            var client = new TokenClient($"{identityUrl}/connect/token", "mvc", "secret");
                            var response = await client.RequestRefreshTokenAsync(refreshToken);

                            if (!response.IsError)
                            {
                                // everything went right, remove old tokens and add new ones
                                identity.RemoveClaim(accessTokenClaim);
                                identity.RemoveClaim(refreshTokenClaim);

                                identity.AddClaims(new[]
                                {
                                        new Claim("access_token", response.AccessToken),
                                        new Claim("refresh_token", response.RefreshToken)
                                });

                                // indicate to the cookie middleware to renew the session cookie
                                // the new lifetime will be the same as the old one, so the alignment
                                // between cookie and access token is preserved
                                x.ShouldRenew = true;
                            }
                        }
                    }
                };
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = identityUrl.ToString();
                options.SignedOutRedirectUri = callBackUrl.ToString();
                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.Scope.Add("api1");
                options.Scope.Add("roles");

                options.ClaimActions.MapJsonKey("role", "role");
                options.ClaimActions.MapJsonKey("team", "team");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };

                options.Events = new OpenIdConnectEvents
                {
                    // that event is called after the OIDC middleware received the auhorisation code,
                    // redeemed it for an access token and a refresh token,
                    // and validated the identity token
                    OnTokenValidated = x =>
                    {
                        // in this handler, TokenEndpointResponse is null!
                        return Task.CompletedTask;
                    },
                    OnTokenResponseReceived = x =>
                    {
                        var identity = (ClaimsIdentity)x.Principal.Identity;
                        identity.AddClaims(new[]
                        {
                                new Claim("access_token", x.TokenEndpointResponse.AccessToken),
                                new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
                        });

                        // so that we don't issue a session cookie but one with a fixed expiration
                        x.Properties.IsPersistent = true;

                        // align expiration of the cookie with expiration of the
                        // access token
                        var accessToken = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken);
                        x.Properties.ExpiresUtc = accessToken.ValidTo;

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IIdentityService, IdentityService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
            //.AddPolicyHandler(GetRetryPolicy())
            //.AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IDataService, DataService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
            //.AddPolicyHandler(GetRetryPolicy())
            //.AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
