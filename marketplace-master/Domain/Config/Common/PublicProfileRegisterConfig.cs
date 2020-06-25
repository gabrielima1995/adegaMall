namespace Application.Config.Common
{
    public class PublicProfileRegisterConfig : RegisterConfig
    {
        public PublicProfileRegisterConfig(bool showInPublicProfile = true, bool enabled = true, bool mandatory = true) : base(enabled, mandatory)
        {
            ShowInPublicProfile = showInPublicProfile;
        }

        public bool ShowInPublicProfile { get; set; }
    }
}
