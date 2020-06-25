using Domain.Config.Common;
using Framework;

namespace Domain.Config.General
{
    public class GdprConfig : IConfigNode
    {
        public bool ShowCookieConsentMessage { get; set; } = false;

        public ErrorDictionary CheckErrors()
        {
            return ErrorDictionary.Empty;
        }

        public ErrorDictionary CheckWarnings()
        {
            return ErrorDictionary.Empty;
        }

        public void Sanitize()
        {
        }
    }
}
