using Application.Config.Common;
using Domain.Config.Common;
using Framework;

namespace Application.Config.General
{
    public class CdnConfig : StatusConfig, IConfigNode
    {
        public string Endpoint { get; set; }

        public new ErrorDictionary CheckErrors()
        {
            var errors = new ErrorDictionary();

            if (Enabled)
            {
                if (Endpoint == null)
                    errors.Add(nameof(Endpoint), "Cannot be null if it is enabled.");

                else if (!ConfigNodeHelper.IsValidUrl(Endpoint))
                    errors.Add(nameof(Endpoint), $"Malformed URL: {Endpoint}");
            }

            errors.Merge(this.CheckChildErrors());
            return errors;
        }

        public new ErrorDictionary CheckWarnings()
        {
            var warnings = new ErrorDictionary();
            warnings.Merge(this.CheckChildWarnings());
            return warnings;
        }

        public new void Sanitize()
        {
            Endpoint = ConfigNodeHelper.SanitizeUrl(Endpoint);
            ConfigNodeHelper.SanitizeSelfAndChildren(this);
        }
    }
}
