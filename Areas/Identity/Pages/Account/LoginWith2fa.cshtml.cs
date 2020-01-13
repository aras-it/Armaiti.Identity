﻿using Armaiti.Identity.Data;
using Armaiti.Identity.Localization;
using Armaiti.Core.Localization.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<ArmaitiUser> _signInManager;
        private readonly IIdentityModelsLocalizer _localizer;
        private readonly ILogger<LoginWith2faModel> _logger;

        public LoginWith2faModel(
            SignInManager<ArmaitiUser> signInManager, 
            IIdentityModelsLocalizer localizer, 
            ILogger<LoginWith2faModel> logger)
        {
            _signInManager = signInManager;
            _localizer = localizer;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [DataType(DataType.Text)]
            [Display(Name = DataAnnotationsResources.AuthenticatorCode)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [StringLength(7, ErrorMessage = DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum, MinimumLength = 6)]
            public string TwoFactorCode { get; set; }

            [Display(Name = DataAnnotationsResources.RememberThisMachine)]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable to load two-factor authentication user."]);
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl ??= Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable to load two-factor authentication user."]);
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, _localizer["Invalid authenticator code."]);
                return Page();
            }
        }  
    }
}