using Armaiti.Core.Localization.Resources;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Armaiti.Identity.Data
{
    /// <summary>
    /// Custom <see cref="IdentityUser"/> implementation with fullname feature.
    /// </summary>
    public class ArmaitiUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the fullname for this user.
        /// </summary>
        [PersonalData]
        [Display(Name = DataAnnotationsResources.FullName)]
        [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
        [StringLength(256, ErrorMessage = DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum, MinimumLength = 5)]
        [RegularExpression(@"^([\p{L}-'`.]+|[\p{L}-'`.]+\s{1}[\p{L}-'`.]{1,}|[\p{L}-'`.]+\s{1}[\p{L}-'`.]{3,}\s{1}[\p{L}-'`.]{1,})$", ErrorMessage = DataAnnotationsResources.FullNameValidation)]
        public string FullName { get; set; }
    }
}
