using Microsoft.Extensions.Localization;
using Rise.Shared.Localization;

namespace Rise.Client.Localization.Register
{
    public class RegisterFormPageResources : IValidatorLocalizer
    {
        private readonly IStringLocalizer<RegisterFormPageResources> _localizer;

        public RegisterFormPageResources(IStringLocalizer<RegisterFormPageResources> localizer)
        {
            _localizer = localizer;
        }

        public string this[string key] => _localizer[key];
    }
}
