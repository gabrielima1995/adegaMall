namespace Application.Config.Common
{
    public class PublicProfileConfig
    {
        public bool ShowInPublicProfile { get; set; }

        public PublicProfileConfig(bool showInPublicProfile = true)
        {
            ShowInPublicProfile = showInPublicProfile;
        }
    }
}
