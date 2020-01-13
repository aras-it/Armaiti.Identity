using Armaiti.Identity.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Armaiti.Identity
{
    /// <summary>
    /// Identity Localizer extensions to <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IdentityLocalizerExtensions
    {
        /// <summary>
        /// Adds a set of common identity services to the application, including UI, localizer, token providers,
        /// and configures authentication to use identity cookies.
        /// </summary>
        /// <remarks>
        /// In order to use the default UI, the application must be using <see cref="Microsoft.AspNetCore.Mvc"/>,
        /// <see cref="Microsoft.AspNetCore.StaticFiles"/> and contain a <c>_LoginPartial</c> partial view that
        /// can be found by the application.
        /// </remarks>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IdentityBuilder"/>.</returns>
        public static IdentityBuilder AddIdentityLocalizer<TUser, TResource>(this IServiceCollection services) where TUser : class where TResource : class
            => services.AddIdentityLocalizer<TUser, TResource>(_ => { });

        /// <summary>
        /// Adds a set of common identity services to the application, including UI, localizer, token providers,
        /// and configures authentication to use identity cookies.
        /// </summary>
        /// <remarks>
        /// In order to use the default UI, the application must be using <see cref="Microsoft.AspNetCore.Mvc"/>,
        /// <see cref="Microsoft.AspNetCore.StaticFiles"/> and contain a <c>_LoginPartial</c> partial view that
        /// can be found by the application.
        /// </remarks>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">Configures the <see cref="IdentityOptions"/>.</param>
        /// <returns>The <see cref="IdentityBuilder"/>.</returns>
        public static IdentityBuilder AddIdentityLocalizer<TUser, TResource>(this IServiceCollection services, Action<IdentityOptions<TResource>> configureOptions) where TUser : class where TResource : class
        {
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(o => { });

            var builder = services
                .AddSingleton<IIdentityModelsLocalizer, IdentityModelsLocalizer<TResource>>()
                .AddSingleton<IIdentityPagesLocalizer, IdentityPagesLocalizer<TResource>>()
                .AddIdentityCore<TUser>(o =>
                {
                    o.Stores.MaxLengthForKeys = 128;
                    configureOptions?.Invoke(new IdentityOptions<TResource>(o));
                })
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<IdentityErrorLocalizer<TResource>>();

            return builder;
        }
    }
}
