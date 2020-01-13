using Microsoft.AspNetCore.Identity;

namespace Armaiti.Identity
{
    /// <summary>
    /// Represents all the options you can use to configure the identity system.
    /// </summary>
    /// <typeparam name="TResource">Type of localizer resource.</typeparam>
    public class IdentityOptions<TResource> : IdentityOptions where TResource : class
    {
        public IdentityOptions(IdentityOptions options)
        {
            ClaimsIdentity = options.ClaimsIdentity;
            Lockout = options.Lockout;
            Password = options.Password;
            SignIn = options.SignIn;
            Stores = options.Stores;
            Tokens = options.Tokens;
            User = options.User;
        }
    }
}
