using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace taggie.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;

        public LogoutModel(SignInManager<IdentityUser> signInManager,
            ILogger<LogoutModel> logger,
            IIdentityServerInteractionService interaction)
        {
            _signInManager = signInManager;
            _logger = logger;
            _interaction = interaction;
        }

        public async Task<IActionResult> OnGet(string logoutId)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var postLogoutRedirectUri = logout?.PostLogoutRedirectUri;

            if (postLogoutRedirectUri != null)
            {
                return Redirect(postLogoutRedirectUri);
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return Page();
            }
        }
    }
}