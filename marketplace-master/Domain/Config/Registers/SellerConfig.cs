using Application.Config.Common;

namespace Application.Config.Registers
{
    public class SellerConfig
    {
        public PublicProfileConfig Email { get; set; } = new PublicProfileConfig(showInPublicProfile: false);

        public PublicProfileConfig Neighborhood { get; set; } = new PublicProfileConfig(showInPublicProfile: true);

        public SellerAddressConfig CityAndState { get; set; } = new SellerAddressConfig(showText: true, showMap: false);

        public SellerAddressConfig Address { get; set; } = new SellerAddressConfig(showText: false, showMap: true);

        public PublicProfileRegisterConfig Phone { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: false);

        public PublicProfileRegisterConfig Genre { get; set; } = new PublicProfileRegisterConfig();

        public DocumentNumberConfig DocumentNumber { get; set; } = new DocumentNumberConfig();

        public SellerDateOfBirthConfig DateOfBirth { get; set; } = new SellerDateOfBirthConfig(showInPublicProfile: false, blockChangesAfterApprovedInModeration: false);

        public PublicProfileRegisterConfig AdditionalInfo1 { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true);

        public PublicProfileRegisterConfig AdditionalInfo2 { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true);

        public PublicProfileRegisterConfig FacebookProfile { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true, mandatory: false);

        public PublicProfileRegisterConfig LinkedInProfile { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true, mandatory: false);

        public PublicProfileRegisterConfig InstagramProfile { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true, mandatory: false);

        public PublicProfileRegisterConfig GooglePlusProfile { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true, mandatory: false);

        public SellerAlternativeNameConfig AlternativeName { get; set; } = new SellerAlternativeNameConfig(showInPublicProfile: true, mandatory: false);

        public PublicProfileRegisterConfig WhatsAppNumber { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true, enabled: false, mandatory: false);

        public PublicProfileRegisterConfig AboutMe { get; set; } = new PublicProfileRegisterConfig(showInPublicProfile: true, mandatory: false);

        public StatusConfig Moderation { get; set; } = new StatusConfig(true);

        public StatusConfig SavePartialProfiles { get; set; } = new StatusConfig(false);

        public StatusConfig UserRatings { get; set; } = new StatusConfig(true);

        public StatusConfig SkipToContent { get; set; } = new StatusConfig(true);

        public StatusConfig RequirePhoneForQuoteRegister { get; set; } = new StatusConfig(false);

        public string MainPageUrl { get; set; } = "/manage";

        public bool ChangeAvatar { get; set; } = true;

        public RegisterConfig Avatar { get; set; } = new RegisterConfig(enabled: true, mandatory: false);

        public SellerAssistantConfig Assistant { get; set; } = new SellerAssistantConfig();
    }

    public class SellerAlternativeNameConfig : PublicProfileRegisterConfig
    {
        public bool GenerateEncoded { get; set; }

        public string EncodedTemplate { get; set; } = "Empresa {0}";

        public SellerAlternativeNameConfig(bool showInPublicProfile = true, bool enabled = true, bool mandatory = true)
            : base(showInPublicProfile, enabled, mandatory)
        {
        }
    }
}