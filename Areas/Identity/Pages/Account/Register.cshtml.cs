using Armaiti.Identity.Localization;
using Armaiti.Core.Localization.Resources;
using Armaiti.Core.Messaging;
using Armaiti.Core.Services;
using Armaiti.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ArmaitiUser> _signInManager;
        private readonly UserManager<ArmaitiUser> _userManager;
        private readonly IIdentityModelsLocalizer _localizer;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailService;

        public RegisterModel(
            UserManager<ArmaitiUser> userManager,
            SignInManager<ArmaitiUser> signInManager,
            IIdentityModelsLocalizer localizer,
            ILogger<RegisterModel> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _logger = logger;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel
        {
            [Display(Name = DataAnnotationsResources.Email)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [EmailAddress(ErrorMessage = DataAnnotationsResources.EmailAddressAttribute_Invalid)]
            public string Email { get; set; }

            [Display(Name = DataAnnotationsResources.FullName)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [StringLength(256, ErrorMessage = DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum, MinimumLength = 5)]
            [RegularExpression(@"^([\p{L}-'`.]+|[\p{L}-'`.]+\s{1}[\p{L}-'`.]{1,}|[\p{L}-'`.]+\s{1}[\p{L}-'`.]{3,}\s{1}[\p{L}-'`.]{1,})$", ErrorMessage = DataAnnotationsResources.FullNameValidation)]
            public string FullName { get; set; } //\p{L} : Unicode characters, \s : space

            [DataType(DataType.Password)]
            [Display(Name = DataAnnotationsResources.Password)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [StringLength(100, ErrorMessage = DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum, MinimumLength = 6)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = DataAnnotationsResources.PasswordValidation)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = DataAnnotationsResources.ConfirmPassword)]
            [Compare("Password", ErrorMessage = DataAnnotationsResources.CompareAttribute_MustMatch)]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ArmaitiUser { FullName = Input.FullName, Email = Input.Email, UserName = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Add a user to the default role, or any role you prefer here
                    await _userManager.AddToRoleAsync(user, RoleSeed.Member);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code },
                        protocol: Request.Scheme);

                    await _emailService.SendAsync(new EmailMessage
                    {
                        Body = _localizer["Please confirm your account by <a href='{0}'>clicking here</a>.", HtmlEncoder.Default.Encode(callbackUrl)],
                        IsBodyHtml = true,
                        MailTo = Input.Email,
                        Subject = _localizer["Confirm your email"]
                    });

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
