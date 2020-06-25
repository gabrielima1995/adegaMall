using Application.Config.Common;

namespace Application.Config.Registers
{
    public class BuyerConfig : StatusConfig
    {
        public RegisterConfig CityAndState { get; set; } = new RegisterConfig();

        public RegisterConfig Address { get; set; } = new RegisterConfig();

        public RegisterConfig Phone { get; set; } = new RegisterConfig();

        public RegisterConfig Genre { get; set; } = new RegisterConfig(mandatory: false);

        public DocumentNumberConfig DocumentNumber { get; set; } = new DocumentNumberConfig();

        public DateOfBirthConfig DateOfBirth { get; set; } = new DateOfBirthConfig(mandatory: false, blockChangesAfterApprovedInModeration: false);

        public StatusConfig Avatar { get; set; } = new StatusConfig();

        public StatusConfig UserRatings { get; set; } = new StatusConfig(true);

        public StatusConfig Moderation { get; set; } = new StatusConfig(false);

        public BuyerCallToActionConfig CallToAction { get; set; } = new BuyerCallToActionConfig();

        public string MainPageUrl { get; set; } = "/manage";

        public bool AllowPlainTextZipCode { get; set; } = false;

        public bool AllowPlainTextCity { get; set; } = false;

        public BuyerConfig()
            : base(enabled: false)
        {
        }
    }

    public class BuyerCallToActionConfig
    {
        public bool ShowSearchButton { get; set; } = false;

        public bool ShowGeneralQuoteButton { get; set; } = false;
    }
}