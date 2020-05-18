using Application.Config;
using Application.Modules.Subscriptions;
using Application.Modules.Subscriptions.PagarMe;
using Application.Modules.Subscriptions.Stripe;
using Domain.Interfaces;
using Domain.Services.StoreModule;
using Infrastructure;
using NewIntegrator.Stripe;
using NewIntegrator.Stripe.Retro.Customers;
using NewIntegrator.Stripe.Retro.Plans;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Core.Subscriptions;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Iugu;
using PaymentIntegrator.Iugu.Webhooks;
using PaymentIntegrator.Moip.Customers;
using PaymentIntegrator.Moip.Suppliers;
using PaymentIntegrator.PagarMe.Customers;
using PaymentIntegrator.PagarMe.PaymentMethods;
using PaymentIntegrator.PagarMe.Payments;
using PaymentIntegrator.PagarMe.Plans;
using PaymentIntegrator.PagarMe.Subscriptions;
using PaymentIntegrator.PagarMe.Suppliers;
using System;
using System.Linq;
using Unity;
using Unity.Lifetime;
using V2.Application.Modules.Finance.Interfaces;
using V2.Application.Modules.Finance.Queries;
using V2.Application.Modules.StoreModule;
using V2.Infrastructure.Modules.Finance;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class PaymentDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container

                // Gateway registration
                .RegisterType<ICustomerOperations, CustomerOperations>()
                .RegisterType<IResetGatewayIds, PagarmeResetGatewayIds>()
                .RegisterType<ISupplierOperations, SupplierOperations>()

                // Payment
                .RegisterType<CreditCardService>()
                .RegisterType<IPaymentService, PaymentService>()
                .RegisterType<IPaymentMethodOperations, PaymentMethodOperations>()
                .RegisterType<IPaymentOperations, PaymentOperations>()
                .RegisterType<CheckStatusForTransactionsPendingPaymentRecurrentJob>()

                // Subscriptions
                .RegisterType<IPlanOperations, PlanOperations>()
                .RegisterType<ISubscriptionOperations, SubscriptionOperations>()
                .RegisterType<ISubscriptionGateway, PagarMeSubscriptionGateway>()
                .RegisterType<PagarmePostbackHelper>();
        }

        public void SetupAfterConfigLoaded(IUnityContainer container)
        {
            UnregisterAll(container);

            if (MaaSConfig.Current.Payment.Enabled)
            {
                var activeGateway = MaaSConfig.Current.Payment.ActiveGateway ?? "";

                switch (activeGateway.ToLower())
                {
                    case "iugu":
                        RegisterIugu(container);
                        break;

                    case "pagarme":
                        RegisterPagarme(container);
                        break;

                    case "moip":
                        RegisterMoip(container);
                        break;

                    case "stripe":
                        RegisterStripe(container);
                        break;

                    default:
                        throw new Exception($"Unknown payment gateway '{activeGateway}'.");
                }
            }
            else
            {
                RegisterDummy(container);
            }
        }

        private void RegisterDummy(IUnityContainer container)
        {
            container.RegisterType<IPaymentGateway, DummyPaymentGateway>();
        }

        private static string GetBankSlipPostbackUrl()
        {
            var config = MaaSConfig.Current;

            var isIugu = "iugu".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);
            var isPagarme = "pagarme".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);

            var baseUri = new Uri(config.Environment.AppUrl);
            var path = isIugu ? "/iugu/boletopostback" : isPagarme ? "/pagarme/boletopostback" : null;

            return new Uri(baseUri, path).ToString();
        }

        private static void RegisterIugu(IUnityContainer container)
        {
            container
                .RegisterType<ICustomerOperations, PaymentIntegrator.Iugu.Customers.CustomerOperations>()
                .RegisterType<IPaymentGateway, IuguPaymentGateway>()
                .RegisterType<IPaymentMethodOperations, PaymentIntegrator.Iugu.PaymentMethods.PaymentMethodOperations>()
                .RegisterType<IPaymentOperations, PaymentIntegrator.Iugu.Payments.PaymentOperations>()
                .RegisterType<IPlanOperations, PaymentIntegrator.Iugu.Plans.PlanOperations>()
                .RegisterType<ISubscriptionOperations, PaymentIntegrator.Iugu.Subscriptions.SubscriptionOperations>()
                .RegisterType<ISupplierOperations, PaymentIntegrator.Iugu.Suppliers.SupplierOperations>();

            var config = MaaSConfig.Current;

            IuguConfig.AccountId = config.Payment.Iugu.AccountId;
            IuguConfig.ApiKeyLive = config.Payment.Iugu.ApiKeyLive;
            IuguConfig.ApiKeyTest = config.Payment.Iugu.ApiKeyTest;
            IuguConfig.BaseUrl = "https://api.iugu.com/v1/";
            IuguConfig.Mode = config.Payment.Iugu.IsLive ? "live" : "test";
            IuguConfig.CommissionPercent = config.Payment.Common.GlobalStartupPercentageFee;
            IuguConfig.SoftDescriptor = config.ApplicationName;
            IuguConfig.PostbackUrl = GetBankSlipPostbackUrl();

            MasterAccountSetup.Configure();
            WeebhookSetup.Configure();
        }

        private static void RegisterPagarme(IUnityContainer container)
        {
            container.RegisterType<ICustomerOperations, CustomerOperations>();
            container.RegisterType<IPaymentGateway, PagarmePaymentGateway>();
            container.RegisterType<IPaymentMethodOperations, PaymentMethodOperations>();
            container.RegisterType<IPaymentOperations, PaymentOperations>();
            container.RegisterType<ISupplierOperations, SupplierOperations>();
            container.RegisterType<IPlanOperations, PlanOperations>();
            container.RegisterType<ISubscriptionOperations, SubscriptionOperations>();
            container.RegisterType<ISubscriptionGateway, PagarMeSubscriptionGateway>();
            container.RegisterType<PagarmePostbackHelper>();

            var config = MaaSConfig.Current;

            PaymentIntegrator.PagarMe.PagarMeConfig.ApiKey = config.Payment.PagarMe.ApiKey;
            PaymentIntegrator.PagarMe.PagarMeConfig.EncryptionKey = config.Payment.PagarMe.EncryptionKey;
            PaymentIntegrator.PagarMe.PagarMeConfig.SoftDescriptor = config.Payment.PagarMe.SoftDescriptor;
            PaymentIntegrator.PagarMe.PagarMeConfig.RecipientChargeProcessingFee = config.Payment.Common.RecipientChargeProcessingFee;
            PaymentIntegrator.PagarMe.PagarMeConfig.RecipientLiable = config.Payment.Common.GlobalIsLiable;
            PaymentIntegrator.PagarMe.PagarMeConfig.RecipientTransferInterval = config.Payment.Common.RecipientTransferInterval;
            PaymentIntegrator.PagarMe.PagarMeConfig.RecipientTransferDay = config.Payment.Common.RecipientTransferDay;
            PaymentIntegrator.PagarMe.PagarMeConfig.RecipientTransferEnabled = config.Payment.Common.RecipientTransferEnabled;
            PaymentIntegrator.PagarMe.PagarMeConfig.StartupPercentageFee = config.Payment.Common.GlobalStartupPercentageFee;
            PaymentIntegrator.PagarMe.PagarMeConfig.StartupAmountFee = config.Payment.Common.GlobalStartupAmountFee;
            PaymentIntegrator.PagarMe.PagarMeConfig.StartupRecipient = config.Payment.PagarMe.StartupRecipient;
            PaymentIntegrator.PagarMe.PagarMeConfig.AntiFraudEnabled = config.Payment.AntiFraud.Enabled;
            PaymentIntegrator.PagarMe.PagarMeConfig.TransferCostCents = (int)(config.Payment.PagarMe.TransferCost * 100);

            PaymentIntegrator.PagarMe.SimpleImpl.Config.ApiKey = config.Payment.PagarMe.ApiKey;
            PaymentIntegrator.PagarMe.SimpleImpl.Config.EncryptionKey = config.Payment.PagarMe.EncryptionKey;
        }

        private static void RegisterMoip(IUnityContainer container)
        {
            container.RegisterType<ICustomerOperations, MoipCustomerOperations>();
            container.RegisterType<IPaymentGateway, MoipPaymentGateway>();
            container.RegisterType<ISupplierOperations, MoipSupplierOperations>();

            container.RegisterType<IPaymentMethodOperations, PaymentIntegrator.Moip.Payments.MoipPaymentMethodOperations>();
            container.RegisterType<IPaymentOperations, PaymentIntegrator.Moip.Payments.MoipPaymentOperations>();

            container.RegisterType<IPlanOperations, PaymentIntegrator.Moip.Plans.MoipPlanOperations>();
            container.RegisterType<ISubscriptionOperations, PaymentIntegrator.Moip.Subscriptions.MoipSubscriptionOperations>();

            var moipConfig = MaaSConfig.Current.Payment.Moip;

            PaymentIntegrator.Moip.MoipConfig.Current.AccountKey = moipConfig.AccountKey;
            PaymentIntegrator.Moip.MoipConfig.Current.AccountToken = moipConfig.AccountToken;
            PaymentIntegrator.Moip.MoipConfig.Current.AppAccessToken = moipConfig.AppAccessToken;
            PaymentIntegrator.Moip.MoipConfig.Current.AppId = moipConfig.AppId;
            PaymentIntegrator.Moip.MoipConfig.Current.AppSecret = moipConfig.AppSecret;
            PaymentIntegrator.Moip.MoipConfig.Current.MarketplaceZeroCommission = moipConfig.MarketplaceZeroCommission;
            PaymentIntegrator.Moip.MoipConfig.Current.UseMarketplaceName = MaaSConfig.Current.Payment.Common.StatementDescriptor.UseMarketplaceName;
            PaymentIntegrator.Moip.MoipConfig.Current.IsSandbox = moipConfig.IsSandbox;
            PaymentIntegrator.Moip.MoipConfig.Current.StartupReceiverId = moipConfig.StartupReceiverId;
            PaymentIntegrator.Moip.MoipConfig.Current.PublicEncryptionKey = moipConfig.PublicEncryptionKey;
            PaymentIntegrator.Moip.MoipConfig.Current.NotificationRule = moipConfig.NotificationRuleId;
            PaymentIntegrator.Moip.MoipConfig.Current.PaymentWarrantyEnabled = MaaSConfig.Current.Payment.Common.EnableTransactionWarranty;
            PaymentIntegrator.Moip.MoipConfig.Current.RecipientChargeProcessingFee = MaaSConfig.Current.Payment.Common.RecipientChargeProcessingFee;
            PaymentIntegrator.Moip.MoipConfig.Current.PostbackUrl = MaaSConfig.Current.Environment.AppUrl;

            PaymentIntegrator.Moip.MoipInitializer.Initialize(new PaymentIntegrator.Moip.MoipInitializationModel(
                $"{MaaSConfig.Current.ApplicationName} Moip App",
                "App do gateway de pagamento",
                MaaSConfig.Current.Environment.AppUrl,
                MaaSConfig.Current.Environment.AppUrl + "moipredirect/"));

            moipConfig.AppId = PaymentIntegrator.Moip.MoipConfig.Current.AppId;
            moipConfig.AppSecret = PaymentIntegrator.Moip.MoipConfig.Current.AppSecret;
            moipConfig.AppAccessToken = PaymentIntegrator.Moip.MoipConfig.Current.AppAccessToken;
            moipConfig.StartupReceiverId = PaymentIntegrator.Moip.MoipConfig.Current.StartupReceiverId;
            moipConfig.NotificationRuleId = PaymentIntegrator.Moip.MoipConfig.Current.NotificationRule;
        }

        private static void RegisterStripe(IUnityContainer container)
        {
            container.RegisterType<ICustomerOperations, StripeRetroCustomerOperations>();
            container.RegisterType<IPlanOperations, StripeRetroPlanOperations>();
            container.RegisterType<ISubscriptionGateway, StripeSubscriptionGateway>();

            var config = MaaSConfig.Current.Payment.Stripe;

            StripeSetup.Init(config.SecretKey, config.PublishableKey);
        }

        private void UnregisterAll(IUnityContainer container)
        {
            UnregisterType<ICustomerOperations>(container);
            UnregisterType<IPaymentGateway>(container);
            UnregisterType<IPaymentMethodOperations>(container);
            UnregisterType<IPaymentOperations>(container);
            UnregisterType<ISupplierOperations>(container);
            UnregisterType<IPlanOperations>(container);
            UnregisterType<ISubscriptionOperations>(container);
        }

        private void UnregisterType<T>(IUnityContainer container)
        {
            var registration = container.Registrations.Where(x =>
                x.RegisteredType == typeof(T)); // &&
                //x.LifetimeManager.LifetimeType == typeof(ContainerControlledLifetimeManager));

            if (!registration.Any()) return;
            registration.Single().LifetimeManager.RemoveValue();
        }
    }
}