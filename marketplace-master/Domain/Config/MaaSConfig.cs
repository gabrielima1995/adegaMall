using Application.Config.General;
using Application.Config.Modules;
using Application.Config.Registers;
using Application.Config.Sections;
using Domain;
using Domain.Config.Common;
using Domain.Config.General;
using Domain.Config.Modules;
using Domain.Config.Sections;
using FluentValidation;
using Framework;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Application.Config
{
    /// <summary>
    /// Configuração do MaaS (Marketplace as a Service).
    /// </summary>
    public class MaaSConfig : IConfigNode
    {
        #region General configs

        public int Version = 1;

        public string ApplicationName { get; set; } = "INA Marketplace";

        public string DefaultCurrency { get; set; } = "R$";

        public AdminCustomizationConfig AdminCustomization { get; set; } = new AdminCustomizationConfig();

        public EnterpriseConfig Enterprise { get; set; } = new EnterpriseConfig();

        public BlogConfig Blog { get; set; } = new BlogConfig();

        public AdditionalLinksConfig[] AdditionalLinks { get; set; } = new AdditionalLinksConfig[3];

        public CdnConfig Cdn { get; set; } = new CdnConfig();

        public FacebookConfig Facebook { get; set; } = new FacebookConfig();

        public IntercomConfig Intercom { get; set; } = new IntercomConfig();

        public LayoutConfig Layout { get; set; } = new LayoutConfig();

        public PaymentConfig Payment { get; set; } = new PaymentConfig();

        public LocalizationConfig Localization { get; set; } = new LocalizationConfig();

        public EnvironmentConfig Environment { get; set; } = new EnvironmentConfig();

        public SearchConfig Search { get; set; } = new SearchConfig();

        public DashboardConfig Dashboard { get; set; } = new DashboardConfig();

        public UserVoiceConfig UserVoice { get; set; } = new UserVoiceConfig();

        public SocialMediaConfig SocialMedia { get; set; } = new SocialMediaConfig();

        public WebAppConfig WebApp { get; set; } = new WebAppConfig();

        public GdprConfig Gdpr { get; set; } = new GdprConfig();

        #endregion General configs

        #region Sections configsF

        public HeroUnitConfig HeroUnit { get; set; } = new HeroUnitConfig { Enabled = true };

        public BannersConfig Banners { get; set; } = new BannersConfig() { Enabled = false };

        public EcommerceSearchConfig EcommerceSearch { get; set; } = new EcommerceSearchConfig() { Enabled = false };

        public EcommerceProductsConfig EcommerceProducts { get; set; } = new EcommerceProductsConfig();

        public HowItWorksConfig HowItWorks { get; set; } = new HowItWorksConfig();

        public HomeCallSellersConfig HomeCallSellers { get; set; } = new HomeCallSellersConfig();

        public HelpQuestionsConfig HelpQuestions { get; set; } = new HelpQuestionsConfig();

        public MiscAreaConfig MiscArea { get; set; } = new MiscAreaConfig();

        public BestSellersConfig BestSellers { get; set; } = new BestSellersConfig();

        public NearbyProductsConfig NearbyProducts { get; set; } = new NearbyProductsConfig();

        #endregion Sections configs

        #region Registers configs

        public SellerConfig Seller { get; set; } = new SellerConfig();

        public BuyerConfig Buyer { get; set; } = new BuyerConfig();

        public SingleUserMultipleProfilesConfig SingleUserMultipleProfiles { get; set; } = new SingleUserMultipleProfilesConfig(true);

        #endregion Registers configs

        #region Modules configs

        public PortfolioConfig Portfolio { get; set; } = new PortfolioConfig();

        public SubscriptionConfig Subscription { get; set; } = new SubscriptionConfig();

        public QuoteConfig Quote { get; set; } = new QuoteConfig();

        public ProductConfig Product { get; set; } = new ProductConfig();

        public FinanceConfig Finance { get; set; } = new FinanceConfig();

        public RatingsConfig Ratings { get; set; } = new RatingsConfig();

        public CustomFieldsConfig CustomFields { get; set; } = new CustomFieldsConfig();

        public BatchImportConfig BatchImport { get; set; } = new BatchImportConfig();

        #endregion Modules configs

        public static MaaSConfig Current { get; private set; }

        public static event EventHandler<EventArgs> Loaded;

        private MaaSConfig()
        {
            for (int i = 0; i < 3; i++)
                AdditionalLinks[i] = new AdditionalLinksConfig(i + 1);
        }

        public static MaaSConfig Empty()=> new MaaSConfig();

        public static void Load(IRepository<ApplicationConfig> appConfigRepository)
        {
            var appConfig = appConfigRepository.Query().FirstOrDefault();
            Load(appConfig);
        }

        public static MaaSConfig FromJson(string configJson)
        {
            return JsonConvert.DeserializeObject<MaaSConfig>(configJson, new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });
        }

        public string ToJson()
        {
            return JsonHelper.ToJson(this);
        }

        public static void Load(ApplicationConfig appConfig)
        {
            Current = appConfig?.MaaSConfig != null
                ? FromJson(appConfig.MaaSConfig)
                : Empty();

            OnLoaded();
        }

        private static void OnLoaded()
        {
            var onLoaded = Loaded;
            onLoaded?.Invoke(Current, EventArgs.Empty);
        }

        public ErrorDictionary CheckErrors()
        {
            var errors = new ErrorDictionary();

            if (ApplicationName == null)
                errors.Add(nameof(ApplicationName), "Cannot be null.");

            if (DefaultCurrency == null)
                errors.Add(nameof(DefaultCurrency), "Cannot be null.");

            errors.Merge(this.CheckChildErrors());
            return errors;
        }

        public ErrorDictionary CheckWarnings()
        {
            var warnings = new ErrorDictionary();
            warnings.Merge(this.CheckChildWarnings());
            return warnings;
        }

        public void Sanitize()
        {
            ConfigNodeHelper.SanitizeSelfAndChildren(this);
        }
    }

    public class MaaSConfigValidator : AbstractValidator<MaaSConfig>
    {
        public MaaSConfigValidator()
        {
            RuleFor(x => x.Product).SetValidator(new ProductConfigValidator());
            RuleFor(x => x.Search).SetValidator(x => new SearchConfigValidator(x));
        }
    }
}