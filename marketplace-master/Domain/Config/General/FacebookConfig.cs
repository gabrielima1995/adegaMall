using Domain.Config.Common;
using Framework;

namespace Application.Config.General
{
    public class FacebookConfig : IConfigNode
    {
        public string PageUrl { get; set; } = Domain.Resources.AppResources.CONFIG_FACEBOOK_DEFAULT_URL;

        public string AppId { get; set; } = Domain.Resources.AppResources.CONFIG_FACEBOOK_DEFAULT_APP_ID;

        public string AppSecret { get; set; } = Domain.Resources.AppResources.CONFIG_FACEBOOK_DEFAULT_APP_SECRET;

        public bool DisableLoginForBuyer { get; set; }

        public bool DisableLoginForSeller { get; set; }

        public ErrorDictionary CheckErrors()
        {
            var errors = new ErrorDictionary();

            if (PageUrl != null && !ConfigNodeHelper.IsValidUrl(PageUrl))
                errors.Add(nameof(PageUrl), "Malformed facebook page url: " + PageUrl);

            errors.Merge(this.CheckChildErrors());
            return errors;
        }

        public ErrorDictionary CheckWarnings()
        {
            var warnings = new ErrorDictionary();
            warnings.Merge(this.CheckChildWarnings());
            return warnings;
        }

        public void Sanitize()
        {
            ConfigNodeHelper.SanitizeSelfAndChildren(this);
        }
    }
}