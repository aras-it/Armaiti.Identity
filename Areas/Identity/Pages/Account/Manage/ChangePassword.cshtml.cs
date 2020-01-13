using Armaiti.Identity.Data;
using Armaiti.Identity.Localization;
using Armaiti.Core.Localization.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ArmaitiUser> _userManager;
        private readonly SignInManager<ArmaitiUser> _signInManager;
        private readonly IIdentityModelsLocalizer _localizer;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<ArmaitiUser> userManager,
            SignInManager<ArmaitiUser> signInManager,
            IIdentityModelsLocalizer localizer,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [DataType(DataType.Password)]
            [Display(Name = DataAnnotationsResources.CurrentPassword)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            public string OldPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = DataAnnotationsResources.NewPassword)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [StringLength(100, ErrorMessage = DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum, MinimumLength = 6)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = DataAnnotationsResources.ConfirmNewPassword)]
            [Compare("NewPassword", ErrorMessage = DataAnnotationsResources.CompareAttribute_MustMatch)]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = _localizer["Your password has been changed."];

            return RedirectToPage();
        }
    }
}
