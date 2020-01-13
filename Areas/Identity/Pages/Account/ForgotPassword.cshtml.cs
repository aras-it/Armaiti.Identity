using Armaiti.Identity.Data;
using Armaiti.Identity.Localization;
using Armaiti.Core.Localization.Resources;
using Armaiti.Core.Messaging;
using Armaiti.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ArmaitiUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IIdentityModelsLocalizer _localizer;

        public ForgotPasswordModel(
            UserManager<ArmaitiUser> userManager, 
            IEmailService emailService, 
            IIdentityModelsLocalizer localizer)
        {
            _userManager = userManager;
            _emailService = emailService;
            _localizer = localizer;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = DataAnnotationsResources.Email)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [EmailAddress(ErrorMessage = DataAnnotationsResources.EmailAddressAttribute_Invalid)]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    StatusMessage = _localizer["Email '{0}' is invalid.", Input.Email];
                    return RedirectToPage("./ForgotPasswordConfirmation"); 
                }
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    StatusMessage = _localizer["Email '{0}' is not confirmed yet.", Input.Email];
                    return RedirectToPage("./ForgotPasswordConfirmation"); 
                }
                //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return RedirectToPage("./ForgotPasswordConfirmation");
                //}

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailService.SendAsync(new EmailMessage
                {
                    MailTo = Input.Email,
                    Subject = _localizer["Reset Password"],
                    Body = _localizer["Please reset your password by <a href='{0}'>clicking here</a>.", HtmlEncoder.Default.Encode(callbackUrl)]
                });

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
