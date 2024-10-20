using Microsoft.Extensions.Localization;

namespace Rise.Client.Localization
{
    public class LandingPageResource
    {
        private readonly IStringLocalizer<LandingPageResource> _localizer;

        public LandingPageResource(IStringLocalizer<LandingPageResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string key] => _localizer[key];
    }
}
