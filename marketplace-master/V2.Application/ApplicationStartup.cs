using Application.Config;
using Domain;
using Domain.Config.ConfigMigrations;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.UserConfig;
using Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using V2.Application.Cities;
using V2.Application.Messages;
using V2.Application.Modules.Finance;
using V2.Application.Modules.LeadSales;
using V2.Application.Modules.Search.Products;
using V2.Application.Modules.StoreModule;
using V2.Application.Modules.UserRatings;
using V2.Application.StartupProcesses;
using V2.Application.Modules.IntegrationParameterFromGoogle;
using FCron = Framework.Cron;
using System.Threading.Tasks;

namespace V2.Application
{
    public class ApplicationStartup
    {
        public static event EventHandler<EventArgs> Loaded;

        private static bool hangfireConfigured = false;
        private Action registerHangfireAction;

        private readonly ConfigMigrator configMigrator;
        private readonly ICloudStorage cloudStorage;
        private readonly IBackgroundWorker backgroundWorker;
        private readonly IRepository<ApplicationConfig> appConfigRepository;
        private readonly IMessageTemplateRepository messageTemplateRepository;
        private readonly ITemplateEngine templateEngine;
        private readonly IEmailSender emailSender;
        private readonly IUserConfigRepository userConfigRepository;
        private readonly IRepository<ProductVariant> productVariantRepository;
        

        private readonly IntegrationParameterFromGoogleService _IntegrationParameterGoogleService;

        public static bool DependenciesLoaded { get; set; }

        public ApplicationStartup(
            ConfigMigrator configMigrator,
            ICloudStorage cloudStorage,
            IBackgroundWorker backgroundWorker,
            IRepository<ApplicationConfig> appConfigRepository,
            IMessageTemplateRepository messageTemplateRepository,
            ITemplateEngine templateEngine,
            IEmailSender emailSender,
            IUserConfigRepository userConfigRepository,
            IRepository<ProductVariant> productVariantRepository,
            IntegrationParameterFromGoogleService IntegrationParameterGoogleService)
        {
            this.configMigrator = configMigrator;
            this.cloudStorage = cloudStorage;
            this.backgroundWorker = backgroundWorker;
            this.appConfigRepository = appConfigRepository;
            this.messageTemplateRepository = messageTemplateRepository;
            this.templateEngine = templateEngine;
            this.emailSender = emailSender;
            this.userConfigRepository = userConfigRepository;
            this.productVariantRepository = productVariantRepository;
            this._IntegrationParameterGoogleService = IntegrationParameterGoogleService;
        }

        public void Startup(Action registerHangfireAction)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            this.registerHangfireAction = registerHangfireAction;

            MaaSConfig.Loaded += MaaSConfig_Loaded;
            MaaSConfig.Load(appConfigRepository);

            configMigrator.EnsureLatestVersion();

            UserConfiguration.Current = userConfigRepository.ReadConfig() ?? new UserConfiguration();

            MessagesContext.Load(messageTemplateRepository, templateEngine, emailSender);

            PaisBadgeExtension.LoadPaises(productVariantRepository);

            backgroundWorker.Enqueue<FriendlyUrlBuilder>(i => i.Run());
            backgroundWorker.Enqueue<SlugifyProductsBuilder>(i => i.Run());
            backgroundWorker.Enqueue<ICityManager>(i => i.PopulateDatabase());
            backgroundWorker.Enqueue<InitGatewayRegistrationLogJob>(i => i.RunIfNeeded());
            backgroundWorker.Schedule<CalculatePriceOfProductsVersionOutJob>(i => i.RunIfNeeded(), TimeSpan.FromMinutes(10));
            backgroundWorker.Schedule<MessageTemplateSeeder>(i => i.PopulateDatabase(), TimeSpan.FromMinutes(1));
            backgroundWorker.Enqueue<SellerProfileValidationService>(i => i.RunLater());
            ExecuteAsync(token);
        }

        private void MaaSConfig_Loaded(object sender, EventArgs e)
        {
            while (!DependenciesLoaded)
                Thread.Sleep(200);

            UpdateAndSaveConfig();
            EnableTlsProtocolForPaymentGateways();
            ConfigureRecurringJobs();

            try
            {
                cloudStorage.DoSetup();
            }
            catch { }

            Loaded?.Invoke(this, EventArgs.Empty);
        }

        private void ConfigureHangfire()
        {
            if (hangfireConfigured)
                return;

            hangfireConfigured = true;
            registerHangfireAction?.Invoke();
        }

        private void ConfigureRecurringJobs()
        {
            ConfigureHangfire();

            var config = MaaSConfig.Current;

            if (config.Payment.Enabled)
            {
                backgroundWorker.CreateRecurring<IntegrationParameterFromGoogleService>(
                    x => x.Run(), "IntegrationParameterGoogleService", FCron.HourInterval(8));

                backgroundWorker.CreateRecurring<CheckStatusForTransactionsPendingPaymentRecurrentJob>(
                    x => x.Run(), "CheckStatusForTransactionsPendingPayment", FCron.HourInterval(1));

                if (config.Payment.Common.EnableTransactionWarranty)
                {
                    backgroundWorker.CreateRecurring<TransactionWarrantyTransferJobs>(
                        x => x.DoScheduledTransfersIfOrderCompleted(), "DoScheduledTransfers", FCron.Daily(2));

                    backgroundWorker.CreateRecurring<TransactionWarrantyTransferJobs>(
                        x => x.AutoCloseOrders(), "DoAutoCloseOrders", FCron.Daily(1));

                    backgroundWorker.CreateRecurring<TransactionWarrantyTransferJobs>(
                        x => x.NotifyUpcomingCompletions(), "UpcomingCompletionNotification", FCron.Daily(3));
                }
            }

            if (config.Product.BusinessModel.Rental.ReminderNotification.Enabled && config.Product.BusinessModel.Rental.Enabled)
                backgroundWorker.CreateRecurring<ReminderNotificationRentalRecurrentJob>(
                    x => x.Run(), "ReminderNotificationRental", FCron.MinuteInterval(15));

            if (config.Quote.LeadSales.Enabled && MaaSConfig.Current.Subscription.Enabled)
                backgroundWorker.CreateRecurring<CheckSubscriptionCredits>(
                    x => x.RunJob(), "CheckScheduledSubscriptionCredits", FCron.Daily(0, 30));

            if (config.Seller.UserRatings.Enabled || config.Buyer.UserRatings.Enabled)
                backgroundWorker.CreateRecurring<CheckExpiredRatings>(x => x.RunJob(), "CheckExpiredRatings", FCron.Daily(4));
        }

        private void EnableTlsProtocolForPaymentGateways()
        {
            var config = MaaSConfig.Current;

            if (!config.Payment.Enabled)
                return;

            if (config.Payment.GatewayIsPagarme() || config.Payment.GatewayIsWirecard() || config.Payment.GatewayIsStripe())
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private void UpdateAndSaveConfig()
        {
            var appConfig = appConfigRepository.Query().FirstOrDefault();
            if (appConfig == null) appConfig = new ApplicationConfig();

            appConfig.MaaSConfig = MaaSConfig.Current.ToJson();

            if (appConfig.Id == 0) appConfigRepository.Insert(appConfig);
            else appConfigRepository.Update(appConfig);

            appConfigRepository.SaveChanges();
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {   
            /*Tarefa vai rodar a cada 4 horas*/
            while (!stoppingToken.IsCancellationRequested)
            {
                 _IntegrationParameterGoogleService.Run();
                await Task.Delay(14400000, stoppingToken); 
            }
        }
    }
}