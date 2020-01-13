using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Armaiti.Identity.Localization
{
    /// <summary>
    /// Generic service to enable localization for application facing identity models.
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    internal class IdentityModelsLocalizer<TResource> : IIdentityModelsLocalizer where TResource : class
    {
        private readonly IStringLocalizer _localizer;

        public IdentityModelsLocalizer(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(TResource));
        }

        public LocalizedString this[string name] => _localizer.GetString(name);

        public LocalizedString this[string name, params object[] arguments] => _localizer.GetString(name, arguments);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        [Obsolete("This method is obsolete. Use `CurrentCulture` and `CurrentUICulture` instead.")]
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return _localizer.WithCulture(culture);
        }
    }
}
