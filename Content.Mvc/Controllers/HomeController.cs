using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Content.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Content.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task Login()
        {
            //var referer = Request.Headers["Referer"].ToString();
            //var isLocal = Url.IsLocalUrl(referer);isLocal ? referer : 

            var returnUrl = Url.Action(nameof(Index), "Home");

            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = returnUrl });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            // "Catalog" because UrlHelper doesn't support nameof() for controllers
            // https://github.com/aspnet/Mvc/issues/5853
            //var homeUrl = Url.Action(nameof(CatalogController.Index), "Catalog");
            //return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme, 
            //    new AspNetCore.Authentication.AuthenticationProperties { RedirectUri = homeUrl });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
