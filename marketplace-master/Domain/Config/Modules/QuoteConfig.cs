using Application.Config.Common;
using Domain.Config.Modules;

namespace Application.Config.Modules
{
    public class QuoteConfig : StatusConfig
    {
        public QuoteConfig() : base(enabled: false)
        {
        }

        public PublicRegisterConfig CityAndState { get; set; } = new PublicRegisterConfig();
        public PublicRegisterConfig Address { get; set; } = new PublicRegisterConfig();
        public PublicRegisterConfig Phone { get; set; } = new PublicRegisterConfig();
        public PublicRegisterConfig Date { get; set; } = new PublicRegisterConfig();
        public PublicRegisterConfig Preference { get; set; } = new PublicRegisterConfig();
        public PublicRegisterConfig QuoteMax { get; set; } = new PublicRegisterConfig();
        public RegisterConfig GeneralQuoteCity { get; set; } = new RegisterConfig(enabled: false, mandatory: false);
        public RegisterConfig PhoneForRegister { get; set; } = new RegisterConfig(enabled: false, mandatory: false);

        public GeneralQuoteConfig GeneralQuote { get; set; } = new GeneralQuoteConfig();
        public SpecificQuoteConfig SpecificQuote { get; set; } = new SpecificQuoteConfig();
        public PortfolioItemQuoteConfig PortfolioItemQuote { get; set; } = new PortfolioItemQuoteConfig();
        public StatusConfig BuyerDecision { get; set; } = new StatusConfig(false);
        public StatusConfig Answers { get; set; } = new StatusConfig(true);
        public StatusConfig Attachment { get; set; } = new StatusConfig(true);
        public StatusConfig Moderation { get; set; } = new StatusConfig(false);

        public bool AnswersFilterContactInfo { get; set; } = false;
        public bool UseBrowserGeolocation { get; set; } = true;
        public bool RejectWithoutAnswer { get; set; } = false;
        public bool ShowBuyerContactInfo { get; set; } = false;
        public bool HideBuyerDetailsUntilDealClosed { get; set; } = false;
        public bool RequirePortfolioForGeneralQuote { get; set; } = false;
        public bool SendQuoteCopyToAdmins { get; set; } = false;
        public bool RequestQuoteToSearchedSellers { get; set; } = true;
        public bool PriceInAnswerRequired { get; set; } = false;

        public LeadSalesConfig LeadSales { get; set; } = new LeadSalesConfig();
        public QuoteBillingConfig QuoteBilling { get; set; } = new QuoteBillingConfig();
    }
}