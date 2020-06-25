namespace Application.Config.General
{
    public class SocialMediaConfig
    {
        public bool EnableShareButtons { get; set; } = true;

        public string ShareThis_Id { get; set; } = Domain.Resources.AppResources.CONFIG_SOCIALMEDIA_DEFAULT_ID;
    }
}