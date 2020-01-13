using Armaiti.Identity.Localization;
using Armaiti.Core.Localization.Resources;
using Armaiti.Core.Services;
using Armaiti.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Armaiti.Identity.Areas.Identity.Pages.Account.Manage
{
    public class RoleAssignmentModel : PageModel
    {
        private readonly UserManager<ArmaitiUser> _userManager;
        private readonly IIdentityModelsLocalizer _localizer;
        private readonly IEmailService _emailService;

        public RoleAssignmentModel(
            UserManager<ArmaitiUser> userManager,
            IIdentityModelsLocalizer localizer,
            IEmailService emailService)
        {
            _userManager = userManager;
            _localizer = localizer;
            _emailService = emailService;
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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !(await _userManager.IsInRoleAsync(user, RoleSeed.Admin)))
            {
                return Unauthorized();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                StatusMessage = _localizer["Unable to load user with email '{0}'.", Input.Email];
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var isManager = await _userManager.IsInRoleAsync(user, RoleSeed.Manager);
            if (!isManager)
            {
                var result = await _userManager.AddToRoleAsync(user, RoleSeed.Manager);
                if (result.Succeeded)
                {
                    await _emailService.SendAsync(
                        Input.Email,
                        _localizer["New role"],
                        _localizer["You are a manager right now."]);
                    return Success();
                }
                else
                {
                    StatusMessage = _localizer["Error assigning role."];
                    return Page();
                }
            }
            else
            {
                return Success();
            }
        }

        private IActionResult Success()
        {
            StatusMessage = _localizer["'{0}' successfully became a manager.", Input.Email];
            Input.Email = "";
            ModelState.Clear();

            return Page();
        }
    }
}
