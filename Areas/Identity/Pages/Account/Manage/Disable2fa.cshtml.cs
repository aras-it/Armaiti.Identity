using Armaiti.Identity.Localization;
using Armaiti.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<ArmaitiUser> _userManager;
        private readonly IIdentityModelsLocalizer _localizer;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(
            UserManager<ArmaitiUser> userManager,
            IIdentityModelsLocalizer localizer,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _localizer = localizer;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException(_localizer["Cannot disable 2FA for user with ID '{0}' as it's not currently enabled.", _userManager.GetUserId(User)]);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException(_localizer["Unexpected error occurred disabling 2FA for user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", _userManager.GetUserId(User));
            StatusMessage = _localizer["2fa has been disabled. You can reenable 2fa when you setup an authenticator app"];
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}