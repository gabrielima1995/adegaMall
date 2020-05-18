using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using System;

namespace Application.Config
{
    public static class ConfigValidator
    {
        public static OperationResult CheckErrors(MaaSConfig config)
        {
            var validation = new ErrorDictionary();

            if (config != null)
            {
                if (string.IsNullOrWhiteSpace(config.Environment.AppUrl))
                    validation.Add("Environment.AppUrl", $"Cannot be null. Essential for system operation.");

                if (string.IsNullOrWhiteSpace(config.Environment.CloudStorage.Endpoint))
                    validation.Add("Environment.CloudStorage.Endpoint",
                        $"Cannot be null. Essential for system operation (will not be able work with cloud storage).");

                if (string.IsNullOrWhiteSpace(config.Environment.CloudStorage.Username))
                    validation.Add("Environment.CloudStorage.Username",
                        $"Cannot be null. Essential for system operation (will not be able work with cloud storage).");

                if (string.IsNullOrWhiteSpace(config.Environment.CloudStorage.Password))
                    validation.Add("Environment.CloudStorage.Password",
                        $"Cannot be null. Essential for system operation (will not be able work with cloud storage).");

                if (string.IsNullOrWhiteSpace(config.Environment.Smtp.Host))
                    validation.Add("Environment.Smtp.Host",
                        $"Cannot be null. Essential for system operation (will not be able to send email messages).");

                if (config.Environment.Smtp.Port == 0)
                    validation.Add("Environment.Smtp.Port",
                        $"Cannot be 0 (zero). Essential for system operation (will not be able to send email messages).");

                if (string.IsNullOrWhiteSpace(config.Environment.Smtp.Username))
                    validation.Add("Environment.Smtp.Username",
                        $"Cannot be null. Essential for system operation (will not be able to send email messages).");

                if (string.IsNullOrWhiteSpace(config.Environment.Smtp.Password))
                    validation.Add("Environment.Smtp.Password",
                        $"Cannot be null. Essential for system operation (will not be able to send email messages).");

                if (string.IsNullOrWhiteSpace(config.Enterprise.CustomerServiceEmail))
                    validation.Add("Enterprise.CustomerServiceEmail",
                        $"Must be a valid email. Essential for system operation (will not be able to send email messages).");

                var isPagarme = "pagarme".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);
                var isIugu = "iugu".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);
                var isMoip = "moip".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);
                var isStripe = "stripe".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);

                if (config.Payment.Enabled)
                {
                    if (!isPagarme && !isIugu && !isMoip && !isStripe)
                        validation.Add("Payment.ActiveGateway",
                            $"Must be set to \"pagarme\", \"iugu\" or \"moip\" because Payment.Enabled is {config.Payment.Enabled}.");

                    if (isStripe)
                    {
                        if (string.IsNullOrWhiteSpace(config.Payment.Stripe.SecretKey))
                            validation.Add("Payment.Stripe.SecretKey",
                                $"Cannot be null because Payment.Enabled is {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");

                        if (string.IsNullOrWhiteSpace(config.Payment.Stripe.PublishableKey))
                            validation.Add("Payment.Stripe.PublishableKey",
                                $"Cannot be null because Payment.Enabled is {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");

                        if (string.IsNullOrWhiteSpace(config.Payment.Stripe.Currency))
                            validation.Add("Payment.Stripe.Currency",
                                $"Cannot be null because Payment.Enabled is {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\". Value must be a supported ISO currency code (https://www.iso.org/iso-4217-currency-codes.html) in lowercase");
                    }

                    if (isPagarme)
                    {
                        if (string.IsNullOrWhiteSpace(config.Payment.PagarMe.ApiKey))
                            validation.Add("Payment.PagarMe.ApiKey",
                                $"Cannot be null because Payment.Enabled is {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");

                        if (string.IsNullOrWhiteSpace(config.Payment.PagarMe.EncryptionKey))
                            validation.Add("Payment.PagarMe.EncryptionKey",
                                $"Cannot be null because Payment.Enabled is {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");

                        if (string.IsNullOrWhiteSpace(config.Payment.PagarMe.StartupRecipient))
                            validation.Add("Payment.PagarMe.StartupRecipient",
                                $"Cannot be null because Payment.Enabled is {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");

                        if (config.Payment.PaymentGateway.Installments.MaxInstallments < 1)
                            validation.Add("Payment.PaymentGateway.Installments.MaxInstallments",
                                $"Cannot be lower than 1.");

                        if (config.Payment.PaymentGateway.Installments.MaxInstallments > 12)
                            validation.Add("Payment.PaymentGateway.Installments.MaxInstallments",
                                $"Cannot be greater than 12.");

                        if (config.Payment.PaymentGateway.Installments.MinOrderAmount < 0M)
                            validation.Add("Payment.PaymentGateway.Installments.MinOrderAmount",
                                $"Cannot be lower than 0 (zero).");

                        if (config.Payment.PaymentGateway.Installments.MinInstallmentAmount < 0M)
                            validation.Add("Payment.PaymentGateway.Installments.MinInstallmentAmount",
                                $"Cannot be lower than 0 (zero).");

                        if (config.Payment.PagarMe.UndergoingValidation && !config.Subscription.Enabled)
                            validation.Add("Payment.PagarMe.UndergoingValidation",
                                $"Subscription module must be enabled.");

                        if (config.Payment.PagarMe.UndergoingValidation &&
                            string.IsNullOrWhiteSpace(config.Payment.PagarMe.ValidationAccountEmail))
                            validation.Add("Payment.PagarMe.ValidationAccountEmail",
                                $"A valid e-mail address is required when undergoing validation");
                    }

                    if (isMoip)
                    {
                        if (!config.Seller.DateOfBirth.Enabled)
                            validation.Add("Seller.DateOfBirth.Enabled",
                                "Seller date of birth is mandatory when using Moip/Wirecard as the payment gateway.");
                    }

                    if (isIugu)
                    {
                        if (string.IsNullOrWhiteSpace(config.Payment.Iugu.AccountId))
                            validation.Add("Payment.Iugu.AccountId",
                                $"Cannot be null because {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");

                        if (string.IsNullOrWhiteSpace(config.Payment.Iugu.ApiKeyLive))
                            validation.Add("Payment.Iugu.ApiKeyLive",
                                $"Cannot be null because {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");

                        if (string.IsNullOrWhiteSpace(config.Payment.Iugu.ApiKeyTest))
                            validation.Add("Payment.Iugu.ApiKeyTest",
                                $"Cannot be null because {config.Payment.Enabled} and Payment.ActiveGateway is set to \"{config.Payment.ActiveGateway}\".");
                    }
                }

                if (config.Portfolio.Enabled)
                {
                    if (config.Portfolio.Search.MapSearchEnabled)
                    {
                        if (string.IsNullOrWhiteSpace(config.Environment.GoogleServices.MapsApiKey))
                            validation.Add("Environment.GoogleServices.MapsApiKey",
                                "If Portfolio.Search.MapSearchEnabled is True, MapsApiKey is mandatory.");
                    }
                }

                if (config.Quote.Enabled)
                {
                    if (config.Portfolio.Enabled == false)
                        validation.Add("Portfolio.Enabled",
                            "If Quote.Enabled is True, Portfolio.Enabled must be True.");

                    if (config.Quote.BuyerDecision.Enabled)
                    {
                        if (config.Buyer.Enabled == false)
                            validation.Add("Quote.Buyer.Enabled",
                                "If Quote.BuyerDecision.Enabled is True, Quote.Buyer.Enabled must be True.");

                        if (config.Quote.Answers.Enabled == false)
                            validation.Add("Quote.Answers.Enabled",
                                "If Quote.BuyerDecision.Enabled is True, Quote.Answers.Enabled must be True.");
                    }

                    if (config.Quote.QuoteBilling.Enabled)
                    {
                        if (config.Quote.BuyerDecision.Enabled == false)
                            validation.Add("Quote.BuyerDecision.Enabled",
                                "If Quote.QuoteBilling.Enabled is True, Quote.BuyerDecision.Enabled must be True.");

                        if (config.Product.Enabled == false)
                            validation.Add("Quote.Product.Enabled",
                                "If Quote.QuoteBilling.Enabled is True, Product.Enabled must be True.");

                        if (config.Product.ActualNoShipping.Enabled == false)
                            validation.Add("Quote.Product.ActualNoShipping.Enabled",
                                "If Quote.QuoteBilling.Enabled is True, Product.ActualNoShipping.Enabled must be True.");

                        if (config.Payment.Enabled == false && config.Product.Order.OfflinePaymentEnabled == false)
                            validation.Add("Quote.Payment.Enabled",
                                "If Quote.QuoteBilling.Enabled is True, Payment.Enabled and/or Product.Order.OfflinePaymentEnabled must be True.");
                    }
                }

                if (config.Product.Enabled)
                {
                    if (config.Product.ShippingMode == ShippingMode.PerProduct)
                    {
                        if (!config.Product.PrivateFleetShipping.Enabled &&
                            !config.Product.OnSitePickupShipping.Enabled && !config.Product.NoShipping.Enabled &&
                            !config.Product.SedexShipping.Enabled && !config.Product.PACShipping.Enabled &&
                            !config.Product.ActualNoShipping.Enabled)
                            validation.Add("Product.ShippingMode",
                                $"Product.ShippingMode is {GetEnumName(ShippingMode.PerProduct)}; enable at least one: Product.PrivateFleetShipping, OnSitePickupShipping, Product.NoShipping, Product.SedexShipping, Product.PACShipping.");

                        if ((config.Product.SedexShipping.Enabled || config.Product.PACShipping.Enabled))
                        {
                            if (!(config.Product.Weight.Enabled && config.Product.Width.Enabled &&
                                  config.Product.Height.Enabled && config.Product.Length.Enabled))
                                validation.Add("Product.Width",
                                    $"If Product.SedexShipping.Enabled is True or Product.PACShipping.Enabled is True, all product dimensions must also be enabled: Weight, Width, Height and Length.");
                        }
                    }

                    if (config.Product.ShippingMode == ShippingMode.PerStore)
                    {
                        if (!config.Product.PrivateFleetShipping.Enabled &&
                            !config.Product.OnSitePickupShipping.Enabled && !config.Product.SedexShipping.Enabled &&
                            !config.Product.PACShipping.Enabled && !config.Product.ActualNoShipping.Enabled)
                            validation.Add("Product.ShippingMode",
                                $"Product.ShippingMode is {GetEnumName(ShippingMode.PerStore)}; enable at least one: Product.PrivateFleetShipping, Product.OnSitePickupShipping, Product.SedexShipping, Product.PACShipping.");
                    }

                    if (!config.Buyer.Enabled)
                    {
                        validation.Add("Buyer.Enabled", $"If Product is {config.Product.Enabled}, enable Buyer.");
                    }

                    if (string.IsNullOrWhiteSpace(config.Buyer.MainPageUrl) || !config.Buyer.MainPageUrl.Contains("/"))
                        validation.Add("Buyer.MainPageUrl", $"Buyer.MainPageUrl is null, empty or not valid main Url {config.Buyer.MainPageUrl}");

                    if (string.IsNullOrWhiteSpace(config.Seller.MainPageUrl) || !config.Seller.MainPageUrl.Contains("/"))
                        validation.Add("Seller.MainPageUrl", $"Seller.MainPageUrl is null, empty or not valid main Url {config.Seller.MainPageUrl}");

                    if (string.IsNullOrWhiteSpace(config.Seller.Assistant.MainPageUrl) || !config.Seller.Assistant.MainPageUrl.Contains("/"))
                        validation.Add("Seller.Assistant.MainPageUrl", $"Seller.Assistant.MainPageUrl is null, empty or not valid main Url {config.Seller.Assistant.MainPageUrl}");

                    if (!config.Payment.Enabled && !config.Product.Order.OfflinePaymentEnabled)
                        validation.Add("Payment.Enabled",
                            $"If Product.Enabled is {config.Product.Enabled}, enable Integrator or Product.Order.OfflinePaymentEnabled.");

                    if (config.Product.BusinessModel.Rental.Enabled)
                    {
                        if (config.Product.BusinessModel.Sale.Enabled)
                            validation.Add("Product.BusinessModel.Rental.Enabled",
                                "Cannot enable both Rental and Sale business models for Product.");

                        if (config.Product.BusinessModel.Rental.GridView.Enabled)
                        {
                            if (config.Product.BusinessModel.Rental.RentalPeriodUnit != RentalPeriodUnit.Hour &&
                                config.Product.BusinessModel.Rental.RentalPeriodUnit != RentalPeriodUnit.Minute)
                                validation.Add("Product.BusinessModel.Rental.RentalPeriodUnit",
                                    $"If Product.BusinessModel.Rental.GridView.Enabled is {config.Product.BusinessModel.Rental.GridView.Enabled}, then Product.BusinessModel.Rental.RentalPeriodUnit must be {RentalPeriodUnit.Hour} or {RentalPeriodUnit.Minute}.");

                            if (!config.Product.BusinessModel.Rental.FixedPeriodRental.Enabled)
                                validation.Add("Product.BusinessModel.Rental.FixedPeriodRental.Enabled",
                                    $"If Product.BusinessModel.Rental.GridView.Enabled is {config.Product.BusinessModel.Rental.GridView.Enabled}, then Product.BusinessModel.Rental.FixedPeriodRental.Enabled must be true.");

                            if (!config.Product.BusinessModel.Rental.FixedPeriodRental.Mandatory)
                                validation.Add("Product.BusinessModel.Rental.FixedPeriodRental.Mandatory",
                                    $"If Product.BusinessModel.Rental.GridView.Enabled is {config.Product.BusinessModel.Rental.GridView.Enabled}, then Product.BusinessModel.Rental.FixedPeriodRental.Mandatory must be true.");

                            if (config.Product.BusinessModel.Rental.RentalPeriodMode != RentalPeriodMode.Unrestricted)
                                validation.Add("Product.BusinessModel.Rental.RentalPeriodMode",
                                    $"If Product.BusinessModel.Rental.GridView.Enabled is {config.Product.BusinessModel.Rental.GridView.Enabled}, then Product.BusinessModel.Rental.RentalPeriodMode must be {RentalPeriodMode.Unrestricted}.");
                        }

                        var rentalPeriodModeResult = RentalPeriodMode.Create(config.Product.BusinessModel.Rental.RentalPeriodMode);

                        if (rentalPeriodModeResult.IsFailure)
                            validation.Add("Product.BusinessModel.Rental.RentalPeriodMode", rentalPeriodModeResult.Errors.ToSimpleString());

                        if (config.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts)
                        {
                            if (config.Product.BusinessModel.Rental.FixedPeriodRental.Enabled)
                                validation.Add("Product.BusinessModel.Rental.FixedPeriodRental",
                                    $"If Produto.BusinessModel.Rental.RentalPeriodMode is {RentalPeriodMode.Shifts}, then Rental.FixedPeriodRental.Enabled must be false.");
                        }
                        else if (config.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Unrestricted)
                        {
                            if (config.Product.BusinessModel.Rental.FixedPeriodRental.IsRequired)
                            {
                                if (config.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.Enabled &&
                                    config.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.MinFixedPeriod <= 0 || config.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.MaxFixedPeriod <= 0)
                                {
                                    validation.Add("Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts",
                                    $"If RentalFixedPeriodForAllProducts.FixedPeriodMin or RentalFixedPeriodForAllProducts.FixedPeriodMax is {0} or lower, then Rental.RentalFixedPeriodForAllProduct.Enabled must be false.");
                                }
                                if(config.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.MinFixedPeriod != config.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.MaxFixedPeriod)
                                {
                                    validation.Add("Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts",
                                    $"If RentalFixedPeriodForAllProducts.FixedPeriodMin is bigger than config.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.FixedPeriodMax, when they should be the same");
                                }
                            }
                        }

                        if (config.Product.BusinessModel.Rental.Package.Enabled)
                        {
                            if (!config.Product.BusinessModel.Rental.FixedPeriodRental.Enabled)
                                validation.Add("Product.BusinessModel.Rental.FixedPeriodRental.Enabled",
                                    $"If Product.BusinessModel.Rental.Package.Enabled is {config.Product.BusinessModel.Rental.Package.Enabled}, then Product.BusinessModel.Rental.FixedPeriodRental.Enabled must be true.");

                            if (!config.Product.BusinessModel.Rental.FixedPeriodRental.Mandatory)
                                validation.Add("Product.BusinessModel.Rental.FixedPeriodRental.Mandatory",
                                    $"If Product.BusinessModel.Rental.Package.Enabled is {config.Product.BusinessModel.Rental.Package.Enabled}, then Product.BusinessModel.Rental.FixedPeriodRental.Mandatory must be true.");
                        }

                        if (config.Product.BusinessModel.Rental.ProductAvailability.Enabled)
                        {
                            if (!config.Product.BusinessModel.Rental.GridView.Enabled)
                                validation.Add("Product.BusinessModel.Rental.GridView.Enabled",
                                    $"If Product.BusinessModel.Rental.ProductAvailability.Enabled is {config.Product.BusinessModel.Rental.ProductAvailability.Enabled}, then Product.BusinessModel.Rental.GridView.Enabled must be true.");

                            if (config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled)
                            {
                                if (!config.Product.BusinessModel.Rental.Package.Enabled)
                                    validation.Add("Product.BusinessModel.Rental.Package.Enabled",
                                        $"If Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled is {config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled}, then Product.BusinessModel.Rental.Package.Enabled must be true.");
                            }
                        }
                    }
                }

                if (config.Subscription.Enabled)
                {
                    if (!config.Payment.Enabled)
                        validation.Add("Payment.Enabled",
                            $"If Subscription.Enabled is {config.Subscription.Enabled}, enable Payment.");
                    else
                    {
                        if (isStripe)
                        {
                            if (string.IsNullOrWhiteSpace(config.Subscription.Stripe.ProductId))
                                validation.Add("Subscription.Stripe.ProductId",
                                    $"If Subscription.Enabled is {config.Subscription.Enabled} and Payment.ActiveGateway is {config.Payment.ActiveGateway}, must be set Subscription.Stripe.ProductId with productId from Subscription product.");
                        }
                    }
                }

                if (config.Finance.PanelEnabled || config.Finance.TransfersEnabled)
                {
                    if (!config.Payment.Enabled || config.Payment.ActiveGateway != "iugu")
                        validation.Add("Payment.Enabled",
                            $"IUGU Integrator must be enabled if Finance.PanelEnabled or Finance.TransfersEnabled are true.");
                }

                if (config.Search.QuotesSearch.Enabled && !config.Quote.Enabled)
                    validation.Add("Quote.Enabled", "Quote must be enabled if Quote Search is enabled");
            }

            return validation;
        }

        public static OperationResult CheckWarnings(MaaSConfig config)
        {
            var validation = new ErrorDictionary();

            if (config != null)
            {
                if (string.IsNullOrWhiteSpace(config.Enterprise.TermsOfUseUrl))
                    validation.Add("Enterprise.TermsOfUseUrl", $"Must be a valid url. This is necessary for the agreement of the terms of use in the user registry.");

                if (string.IsNullOrWhiteSpace(config.Environment.GoogleServices.AnalyticsAccountId))
                    validation.Add("Environment.GoogleServices.AnalyticsAccountId", $"User navigation will not be tracked.");

                if (string.IsNullOrWhiteSpace(config.Environment.GoogleServices.MapsApiKey))
                    validation.Add("Environment.GoogleServices.MapsApiKey", $"Addresses will not be geolocated (no {{lat,long}} coordinates, no maps, no markers).");

                if (!config.Product.Enabled && !config.Portfolio.Enabled)
                    validation.Add("Configuration", $"No module enabled: {nameof(config.Product)}, {nameof(config.Portfolio)}.");

                if (config.Portfolio.Enabled)
                {
                    if (!config.Quote.Enabled)
                        validation.Add("Quote", $"{nameof(config.Portfolio)} is enabled, but {nameof(config.Quote)} is not.");
                }

                if (config.Payment.Enabled)
                {
                    var isPagarme = "pagarme".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);
                    var isIugu = "iugu".Equals(config.Payment.ActiveGateway, StringComparison.OrdinalIgnoreCase);

                    if (isIugu)
                    {
                        if (config.Payment.Iugu.IsLive == false)
                            validation.Add("Payment.Iugu.IsLive", "Is set to False. Remember to turn it ON before going to production.");
                    }
                }

                if (config.Quote.Enabled)
                {
                    if (config.Quote.QuoteBilling.Enabled)
                    {
                        if (config.Product.HideStoreMenusFromSeller == false)
                            validation.Add("Product.HideStoreMenusFromSeller", "Quote.Billing.Enabled is True; you may want to turn this on.");
                    }
                }
            }

            return validation;
        }

        private static string GetEnumName(Enum enumValue)
        {
            var type = enumValue.GetType();
            return Enum.GetName(type, enumValue);
        }
    }
}