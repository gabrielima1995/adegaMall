using Application.Helpers;

namespace Application.StaticPages
{
    public class StaticPagesAppService : IStaticPagesAppService
    {
        public string PrivacyPolicy(string lang)
        {
            return ResourcesHelper.GetStringLocalized("PrivacyPolicy", lang);
        }

        public string HowItWorks(string lang)
        {
            return ResourcesHelper.GetStringLocalized("HowItWorks", lang);
        }

        public string HelpMeToFind(string lang)
        {
            return ResourcesHelper.GetStringLocalized("HelpMeToFind", lang);
        }
    }
}