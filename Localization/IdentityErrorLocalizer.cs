using Armaiti.Identity.Localization.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Armaiti.Identity.Localization
{
    /// <summary>
    /// Generic service to enable localization for application facing identity errors.
    /// </summary>
    /// <typeparam name="TResource">Type of resource.</typeparam>
    internal class IdentityErrorLocalizer<TResource> : IdentityErrorDescriber where TResource : class
    {
        private readonly IStringLocalizer _localizer;

        public IdentityErrorLocalizer(IStringLocalizerFactory factory)
        {
            var resourceType = typeof(TResource);
            var assemblyName = new AssemblyName(resourceType.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create(resourceType.Name, assemblyName.Name);
        }

        public override IdentityError DuplicateEmail(string email)
            => GetLoclizedError(IdentityResources.DuplicateEmail, email);

        public override IdentityError DuplicateUserName(string userName)
            => GetLoclizedError(IdentityResources.DuplicateUserName, userName);

        public override IdentityError InvalidEmail(string email)
            => GetLoclizedError(IdentityResources.InvalidEmail, email);

        public override IdentityError DuplicateRoleName(string role)
            => GetLoclizedError(IdentityResources.DuplicateRoleName, role);

        public override IdentityError InvalidRoleName(string role)
            => GetLoclizedError(IdentityResources.InvalidRoleName, role);

        public override IdentityError InvalidToken()
            => GetLoclizedError(IdentityResources.InvalidToken);

        public override IdentityError InvalidUserName(string userName)
            => GetLoclizedError(IdentityResources.InvalidUserName, userName);

        public override IdentityError LoginAlreadyAssociated()
            => GetLoclizedError(IdentityResources.LoginAlreadyAssociated);

        public override IdentityError PasswordMismatch()
            => GetLoclizedError(IdentityResources.PasswordMismatch);

        public override IdentityError PasswordRequiresDigit()
            => GetLoclizedError(IdentityResources.PasswordRequiresDigit);

        public override IdentityError PasswordRequiresLower()
            => GetLoclizedError(IdentityResources.PasswordRequiresLower);

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => GetLoclizedError(IdentityResources.PasswordRequiresNonAlphanumeric);

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => GetLoclizedError(IdentityResources.PasswordRequiresUniqueChars, uniqueChars);

        public override IdentityError PasswordRequiresUpper()
            => GetLoclizedError(IdentityResources.PasswordRequiresUpper);

        public override IdentityError PasswordTooShort(int length)
            => GetLoclizedError(IdentityResources.PasswordTooShort, length);

        public override IdentityError UserAlreadyHasPassword()
            => GetLoclizedError(IdentityResources.UserAlreadyHasPassword);

        public override IdentityError UserAlreadyInRole(string role)
            => GetLoclizedError(IdentityResources.UserAlreadyInRole, role);

        public override IdentityError UserNotInRole(string role)
            => GetLoclizedError(IdentityResources.UserNotInRole, role);

        public override IdentityError UserLockoutNotEnabled()
            => GetLoclizedError(IdentityResources.UserLockoutNotEnabled);

        public override IdentityError RecoveryCodeRedemptionFailed()
            => GetLoclizedError(IdentityResources.RecoveryCodeRedemptionFailed);

        public override IdentityError ConcurrencyFailure()
            => GetLoclizedError(IdentityResources.ConcurrencyFailure);

        public override IdentityError DefaultError()
            => GetLoclizedError(IdentityResources.DefaultError);

        private IdentityError GetLoclizedError(string code, params object[] args)
        {
            var msg = _localizer[code, args];

            return new IdentityError { Code = code, Description = msg };
        }
    }
}
