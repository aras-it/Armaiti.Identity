using Armaiti.Identity.Localization;
using Armaiti.Core.Localization.Resources;
using Armaiti.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ArmaitiUser> _userManager;
        private readonly SignInManager<ArmaitiUser> _signInManager;
        private readonly IIdentityModelsLocalizer _localizer;

        public IndexModel(
            UserManager<ArmaitiUser> userManager,
            SignInManager<ArmaitiUser> signInManager,
            IIdentityModelsLocalizer localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        [Display(Name = DataAnnotationsResources.Username)]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = DataAnnotationsResources.FullName)]
            [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
            [StringLength(256, ErrorMessage = DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum, MinimumLength = 5)]
            [RegularExpression(@"^([\p{L}-'`.]+|[\p{L}-'`.]+\s{1}[\p{L}-'`.]{1,}|[\p{L}-'`.]+\s{1}[\p{L}-'`.]{3,}\s{1}[\p{L}-'`.]{1,})$", ErrorMessage = DataAnnotationsResources.FullNameValidation)]
            public string FullName { get; set; } //\p{L} : Unicode characters, \s : space

            [Display(Name = DataAnnotationsResources.PhoneNumber)]
            [Phone(ErrorMessage = DataAnnotationsResources.PhoneAttribute_Invalid)]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            Username = user.UserName;

            Input = new InputModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException(_localizer["Unexpected error occurred setting phone number for user with ID '{0}'.", userId]);
                }
            }

            if (Input.FullName != user.FullName)
            {
                user.FullName = Input.FullName;
                var setFullNameResult = await _userManager.UpdateAsync(user);
                if (!setFullNameResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException(_localizer["Unexpected error occurred setting full name for user with ID '{0}'.", userId]);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = _localizer["Your profile has been updated"];
            return RedirectToPage();
        }
    }
}
