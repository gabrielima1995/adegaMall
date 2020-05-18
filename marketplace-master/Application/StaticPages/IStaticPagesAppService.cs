namespace Application.StaticPages
{
    public interface IStaticPagesAppService
    {
        string PrivacyPolicy(string lang);

        string HowItWorks(string lang);

        string HelpMeToFind(string lang);
    }
}