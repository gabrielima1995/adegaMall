namespace Application.Config.Common
{
    public class SellerDateOfBirthConfig : PublicProfileRegisterConfig
    {
        public SellerDateOfBirthConfig(bool showInPublicProfile = true, bool enabled = true, bool mandatory = true, bool blockChangesAfterApprovedInModeration = false) : base(showInPublicProfile, enabled, mandatory)
        {
            BlockChangesAfterApprovedInModeration = blockChangesAfterApprovedInModeration;
        }

        public int MinAge { get; set; } = 0;
        public int MaxAge { get; set; } = 100;
        public bool BlockChangesAfterApprovedInModeration { get; set; } = false;
    }
}
