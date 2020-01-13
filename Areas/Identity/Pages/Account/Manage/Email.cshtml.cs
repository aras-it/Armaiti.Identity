using Armaiti.Identity.Localization;
using Armaiti.Core.Localization.Resources;
using Armaiti.Core.Services;
using Armaiti.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<ArmaitiUser> _userManager;
        private readonly IIdentityModelsLocalizer _localizer;
        private readonly IEmailService _emailService;

        public EmailModel(
            UserManager<ArmaitiUser> userManager,
            IIdentityModelsLocalizer localizer,
            IEmailService emailService)
        {
            _userManager = userManager;
            _localizer = localizer;
            _emailService = emailService;
        }

        [Display(Name = DataAnnotationsResources.Username)]
        public string Username { get; set; }

        [Display(Name = DataAnnotationsResources.Email)]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = DataAnnotationsResources.NewEmail)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [EmailAddress(ErrorMessage = DataAnnotationsResources.EmailAddressAttribute_Invalid)]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(ArmaitiUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId, email = Input.NewEmail, code },
                    protocol: Request.Scheme);
                await _emailService.SendAsync(
                    Input.NewEmail,
                    _localizer["Confirm your email"],
                    _localizer["Please confirm your account by <a href='{0}'>clicking here</a>.", HtmlEncoder.Default.Encode(callbackUrl)]);

                StatusMessage = _localizer["Confirmation link to change email sent. Please check your email."];
                return RedirectToPage();
            }

            StatusMessage = _localizer["Your email is unchanged."];
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
                protocol: Request.Scheme);
            await _emailService.SendAsync(
                    email,
                    _localizer["Confirm your email"],
                    _localizer["Please confirm your account by <a href='{0}'>clicking here</a>.", HtmlEncoder.Default.Encode(callbackUrl)]);

            StatusMessage = _localizer["Verification email sent. Please check your email."];
            return RedirectToPage();
        }
    }
}
