using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Armaiti.Identity.Localization
{
    /// <summary>
    /// Generic service to enable localization for application facing identity pages.
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    internal class IdentityPagesLocalizer<TResource> : IIdentityPagesLocalizer where TResource : class
    {
        private readonly IHtmlLocalizer _localizer;

        public IdentityPagesLocalizer(IHtmlLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(TResource));
        }

        public LocalizedHtmlString this[string name] => _localizer[name];

        public LocalizedHtmlString this[string name, params object[] arguments] => _localizer[name, arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        public LocalizedString GetString(string name)
        {
            return _localizer.GetString(name);
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            return _localizer.GetString(name, arguments);
        }

        [Obsolete("This method is obsolete. Use `CurrentCulture` and `CurrentUICulture` instead.")]
        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            return _localizer.WithCulture(culture);
        }
    }
}
