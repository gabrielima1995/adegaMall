using Application.Config;
using Application.Modules.Finance;
using Domain;
using Domain.Core;
using Domain.Events.Config;
using Framework;
using Presentation.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeMaster]
    public class ConfigController : Controller
    {
        private readonly IRepository<ApplicationConfig> _appConfigRepository;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly ResendGatewayIds _gatewayIdResetter;

        public ConfigController(IRepository<ApplicationConfig> appConfigRepository,
                                IDomainEventDispatcher domainEventDispatcher,
                                ResendGatewayIds gatewayIdResetter)
        {
            _appConfigRepository = appConfigRepository;
            _domainEventDispatcher = domainEventDispatcher;
            _gatewayIdResetter = gatewayIdResetter;
        }

        public ActionResult Index()
        {
            var model = new ConfigViewModel(MaaSConfig.Current.ToJson());
            model.Warnings = ConfigValidator.CheckWarnings(MaaSConfig.Current).Errors;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newConfig = model.MaaSConfig.FromJson<MaaSConfig>();

                model.Errors = ConfigValidator.CheckErrors(newConfig).Errors;

                if (!model.Errors.HasErrors)
                {
                    var appConfig = _appConfigRepository.Query().FirstOrDefault();

                    if (appConfig == null) appConfig = new ApplicationConfig();

                    var oldConfig = appConfig.GetConfig();

                    appConfig.MaaSConfig = model.MaaSConfig;

                    if (appConfig.Id == 0) _appConfigRepository.Insert(appConfig);
                    else _appConfigRepository.Update(appConfig);

                    _appConfigRepository.SaveChanges();

                    MaaSConfig.Load(appConfig);
                    _domainEventDispatcher.Dispatch(new ConfigUpdatedEvent(MaaSConfig.Current, oldConfig));

                    if (!MaaSConfig.Current.UserVoice.Configured)
                    {
                        MaaSConfig.Current.UserVoice.ConfigureUserVoice(false);
                        if (MaaSConfig.Current.UserVoice.Configured)
                        {
                            appConfig.MaaSConfig = MaaSConfig.Current.ToJson();
                            _appConfigRepository.Update(appConfig);
                            _appConfigRepository.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index", new { ok = 1 });
                }
            }

            return View(model);
        }

        public ActionResult BusinessModel()
        {
            return View("BusinessModel", -1);
        }

        [HttpPost]
        public ActionResult ConfigureBusinessModel(int model_id)
        {
            switch (model_id)
            {
                case 1:
                    ConfigureProductSales();
                    break;

                case 2:
                    ConfigureProductRentals();
                    break;

                case 3:
                    ConfigureServiceScheduling();
                    break;

                case 4:
                    ConfigureQuoteNoPayment();
                    break;

                case 5:
                    ConfigureQuoteWithPayment();
                    break;

                case 6:
                    ConfigureQuoteWithLeadSales();
                    break;

                default:
                    throw new InvalidOperationException("The selected business model does not exist");
            }

            SaveConfig(MaaSConfig.Current);

            return View("Success");
        }

        private void ConfigureQuoteNoPayment()
        {
            MaaSConfig config = MaaSConfig.Current;

            config.Quote.Enabled = true;

            config.Quote.GeneralQuote.Enabled = true;
            config.Quote.SpecificQuote.Enabled = true;
            config.Quote.QuoteBilling.Enabled = false;
            config.Quote.BuyerDecision.Enabled = false;
            config.Quote.LeadSales.Enabled = false;

            config.Quote.PortfolioItemQuote.Enabled = true;
            config.Quote.PortfolioItemQuote.Enabled = true;
            config.Quote.Moderation.Enabled = true;
            config.Quote.AnswersFilterContactInfo = true;
            config.Quote.HideBuyerDetailsUntilDealClosed = true;
            config.Quote.SendQuoteCopyToAdmins = true;
            config.Quote.GeneralQuote.RegisterQuoteEvenWhenNoSellers = true;
            config.Quote.GeneralQuote.ShowSellersWithoutAnswer = false;
            config.Quote.PhoneForRegister.Enabled = true;
            config.Quote.PhoneForRegister.Mandatory = true;
            config.Quote.GeneralQuoteCity.Enabled = true;
            config.Portfolio.ShowOwnPage = true;
            config.Portfolio.PortfolioItems.Enabled = true;
            config.Portfolio.ShowRelatedItems = true;
            config.Portfolio.Price.Mandatory = false;
            config.Search.QuotesSearch.Enabled = true;
            config.Buyer.CallToAction.ShowGeneralQuoteButton = true;
            config.Buyer.CallToAction.ShowSearchButton = true;

            config.Payment.Enabled = true;
            config.Subscription.Enabled = true;
            config.Subscription.Mandatory = true;

            config.Product.Enabled = false;

            config.EcommerceSearch.Enabled = false;

            config.Search.Enabled = true;
            config.Search.SearchType = Domain.Config.General.SearchType.Seller;
            config.Search.ResultType = Domain.Config.General.ResultType.Seller;
            config.Search.Map.Enabled = true;
            config.Search.List.Enabled = true;
            config.Search.ShowDescription = true;

            config.Portfolio.Enabled = true;

            config.Buyer.Enabled = true;

            config.CustomFields.Buyer.Enabled = true;
            config.CustomFields.Seller.Enabled = true;
            config.CustomFields.Portfolio.Enabled = true;
            config.CustomFields.Quotes.Enabled = true;
        }

        private void ConfigureQuoteWithPayment()
        {
            MaaSConfig config = MaaSConfig.Current;

            config.Quote.Enabled = true;

            config.Quote.GeneralQuote.Enabled = true;
            config.Quote.SpecificQuote.Enabled = true;
            config.Quote.QuoteBilling.Enabled = true;
            config.Quote.BuyerDecision.Enabled = true;
            config.Quote.LeadSales.Enabled = false;

            config.Quote.PortfolioItemQuote.Enabled = true;
            config.Quote.PortfolioItemQuote.Enabled = true;
            config.Quote.Moderation.Enabled = true;
            config.Quote.AnswersFilterContactInfo = true;
            config.Quote.HideBuyerDetailsUntilDealClosed = true;
            config.Quote.SendQuoteCopyToAdmins = true;
            config.Quote.GeneralQuote.RegisterQuoteEvenWhenNoSellers = true;
            config.Quote.GeneralQuote.ShowSellersWithoutAnswer = false;
            config.Quote.PhoneForRegister.Enabled = true;
            config.Quote.PhoneForRegister.Mandatory = true;
            config.Quote.GeneralQuoteCity.Enabled = true;
            config.Portfolio.ShowOwnPage = true;
            config.Portfolio.PortfolioItems.Enabled = true;
            config.Portfolio.ShowRelatedItems = true;
            config.Portfolio.Price.Mandatory = false;
            config.Search.QuotesSearch.Enabled = true;
            config.Buyer.CallToAction.ShowGeneralQuoteButton = true;
            config.Buyer.CallToAction.ShowSearchButton = true;
            config.Seller.AlternativeName.Enabled = true;
            config.Seller.AlternativeName.GenerateEncoded = true;
            config.Seller.AlternativeName.ShowInPublicProfile = true;
            config.Seller.ChangeAvatar = false;

            config.Layout.TransactionWarrantyDisplay.ShowReassuringMessages = true;
            config.Payment.Common.EnableTransactionWarranty = true;

            config.Payment.Enabled = true;
            config.Subscription.Enabled = false;
            config.EcommerceSearch.Enabled = false;

            config.Product.Enabled = true;
            config.Product.HideStoreMenusFromSeller = true;
            config.Product.ActualNoShipping.Enabled = true;
            config.Product.NoShipping.Enabled = false;
            config.Product.PACShipping.Enabled = false;
            config.Product.SedexShipping.Enabled = false;
            config.Product.PrivateFleetShipping.Enabled = false;
            config.Product.OnSitePickupShipping.Enabled = false;

            config.Search.Enabled = true;
            config.Search.SearchType = Domain.Config.General.SearchType.Seller;
            config.Search.ResultType = Domain.Config.General.ResultType.Seller;
            config.Search.Map.Enabled = true;
            config.Search.List.Enabled = true;
            config.Search.ShowDescription = true;

            config.Portfolio.Enabled = true;

            config.Buyer.Enabled = true;

            config.CustomFields.Buyer.Enabled = true;
            config.CustomFields.Seller.Enabled = true;
            config.CustomFields.Portfolio.Enabled = true;
            config.CustomFields.Quotes.Enabled = true;
        }

        private void ConfigureQuoteWithLeadSales()
        {
            MaaSConfig config = MaaSConfig.Current;

            config.Quote.Enabled = true;

            config.Quote.GeneralQuote.Enabled = true;
            config.Quote.SpecificQuote.Enabled = true;
            config.Quote.QuoteBilling.Enabled = true;
            config.Quote.BuyerDecision.Enabled = true;
            config.Quote.LeadSales.Enabled = true;
            config.Quote.LeadSales.IndividualPacks.Enabled = true;

            config.Payment.Enabled = true;

            config.Subscription.Enabled = true;
            config.Subscription.Mandatory = false;

            config.Quote.PortfolioItemQuote.Enabled = true;
            config.Quote.PortfolioItemQuote.Enabled = true;
            config.Quote.Moderation.Enabled = true;
            config.Quote.AnswersFilterContactInfo = true;
            config.Quote.HideBuyerDetailsUntilDealClosed = true;
            config.Quote.SendQuoteCopyToAdmins = true;
            config.Quote.GeneralQuote.RegisterQuoteEvenWhenNoSellers = true;
            config.Quote.GeneralQuote.ShowSellersWithoutAnswer = false;
            config.Quote.PhoneForRegister.Enabled = true;
            config.Quote.PhoneForRegister.Mandatory = true;
            config.Quote.GeneralQuoteCity.Enabled = true;
            config.Portfolio.ShowOwnPage = true;
            config.Portfolio.PortfolioItems.Enabled = true;
            config.Portfolio.ShowRelatedItems = true;
            config.Portfolio.Price.Mandatory = false;
            config.Search.QuotesSearch.Enabled = true;
            config.Buyer.CallToAction.ShowGeneralQuoteButton = true;
            config.Buyer.CallToAction.ShowSearchButton = true;
            config.Seller.AlternativeName.Enabled = true;
            config.Seller.AlternativeName.GenerateEncoded = true;
            config.Seller.AlternativeName.ShowInPublicProfile = true;
            config.Seller.ChangeAvatar = false;

            config.Product.Enabled = true;
            config.Product.HideStoreMenusFromSeller = true;
            config.Product.ActualNoShipping.Enabled = true;
            config.Product.NoShipping.Enabled = false;
            config.Product.PACShipping.Enabled = false;
            config.Product.SedexShipping.Enabled = false;
            config.Product.PrivateFleetShipping.Enabled = false;
            config.Product.OnSitePickupShipping.Enabled = false;

            config.EcommerceSearch.Enabled = false;

            config.Search.Enabled = true;
            config.Search.SearchType = Domain.Config.General.SearchType.Seller;
            config.Search.ResultType = Domain.Config.General.ResultType.Seller;
            config.Search.Map.Enabled = true;
            config.Search.List.Enabled = true;
            config.Search.ShowDescription = true;

            config.Portfolio.Enabled = true;

            config.Buyer.Enabled = true;

            config.CustomFields.Buyer.Enabled = true;
            config.CustomFields.Seller.Enabled = true;
            config.CustomFields.Portfolio.Enabled = true;
            config.CustomFields.Quotes.Enabled = true;
        }

        private void ConfigureProductSales()
        {
            MaaSConfig config = MaaSConfig.Current;

            config.Quote.Enabled = false;

            config.Product.Enabled = true;
            config.Product.BusinessModel.Rental.Enabled = false;
            config.Product.BusinessModel.Sale.Enabled = true;
            config.Product.Page.Quantity.Enabled = true;
            config.Product.CanPauseProducts = true;
            config.Product.ShowRelatedProducts = true;
            config.Product.Variants.StockByVariantEnabled = true;
            config.Product.Wishlist = true;

            config.Product.Order.SendOrderCopyToAdmins = true;
            config.Product.Order.ExtraInfoField.Enabled = true;

            config.Product.Templates.Enabled = true;
            config.Product.Templates.ForceSellerToUseTemplates = false;

            config.Product.Variants.Enabled = true;

            config.Product.ActualNoShipping.Enabled = false;
            config.Product.PACShipping.Enabled = true;
            config.Product.SedexShipping.Enabled = true;
            config.Product.PrivateFleetShipping.Enabled = true;
            config.Product.OnSitePickupShipping.Enabled = true;

            config.Search.Enabled = true;
            config.Search.ResultType = Domain.Config.General.ResultType.Product;
            config.Search.SearchType = Domain.Config.General.SearchType.Product;
            config.Search.Map.Enabled = false;
            config.Search.List.Enabled = true;
            config.Search.ShowDescription = false;
            config.Search.SearchFields.Query = true;
            config.Search.SearchFields.Location = true;
            config.Search.SearchFields.OnlyOnSitePickup = false;
            config.Search.SearchFields.Category = true;

            config.BestSellers.Enabled = false;
            config.Portfolio.Enabled = false;
            config.Subscription.Enabled = false;

            config.CustomFields.Product.Enabled = true;
            config.CustomFields.Seller.Enabled = true;
            config.CustomFields.Buyer.Enabled = true;

            config.Buyer.Enabled = true;
            config.Buyer.UserRatings.Enabled = false;

            config.Seller.UserRatings.Enabled = true;
        }

        private void ConfigureProductRentals()
        {
            MaaSConfig config = MaaSConfig.Current;

            config.Quote.Enabled = false;

            config.Product.Enabled = true;
            config.Product.BusinessModel.Rental.Enabled = true;
            config.Product.BusinessModel.Rental.EnablePeriodPerProduct = true;
            config.Product.BusinessModel.Rental.QuantityEnabled = true;
            config.Product.BusinessModel.Rental.RentalPeriodUnit = Domain.StoreModule.Entities.RentalPeriodUnit.Day;
            config.Product.BusinessModel.Rental.EnablePriceMultiplier = false;
            config.Product.BusinessModel.Rental.FixedPeriodRental.Enabled = false;
            config.Product.BusinessModel.Rental.ReminderNotification.Enabled = true;
            config.Product.BusinessModel.Rental.MinRentalAmount = true;
            config.Product.BusinessModel.Rental.EnablePriceMultiplier = true;
            config.Product.BusinessModel.Rental.ProductAvailability.Enabled = true;
            config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled = true;
            config.Product.BusinessModel.Rental.Buyer.Cancel.Enabled = true;
            config.Product.BusinessModel.Rental.Buyer.Schedule.Enabled = true;
            config.Product.BusinessModel.Rental.Buyer.Schedule.MinTime = "06:00";
            config.Product.BusinessModel.Rental.Buyer.Schedule.MaxTime = "22:00";
            config.Product.BusinessModel.Rental.Buyer.ChangePeriod.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.BatchLocks.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.Availability.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.Profile.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.SingleLocks.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.MinTime = "06:00";
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.MaxTime = "22:00";
            config.Product.BusinessModel.Sale.Enabled = false;

            config.Product.Page.Quantity.Enabled = true;
            config.Product.Variants.Enabled = true;
            config.Product.Variants.StockByVariantEnabled = true;
            config.Product.CanPauseProducts = true;
            config.Product.ShowRelatedProducts = true;
            config.Product.Wishlist = true;
            config.Product.ActualNoShipping.Enabled = false;
            config.Product.PACShipping.Enabled = true;
            config.Product.SedexShipping.Enabled = true;
            config.Product.PrivateFleetShipping.Enabled = true;
            config.Product.OnSitePickupShipping.Enabled = true;

            config.Product.Order.SendOrderCopyToAdmins = true;
            config.Product.Order.ExtraInfoField.Enabled = true;

            config.Product.Templates.Enabled = true;
            config.Product.Templates.ForceSellerToUseTemplates = false;

            config.Search.Enabled = true;
            config.Search.ResultType = Domain.Config.General.ResultType.Product;
            config.Search.SearchType = Domain.Config.General.SearchType.Product;
            config.Search.Map.Enabled = false;
            config.Search.List.Enabled = true;
            config.Search.ShowDescription = false;
            config.Search.SearchFields.Query = true;
            config.Search.SearchFields.Location = true;
            config.Search.SearchFields.OnlyOnSitePickup = false;
            config.Search.SearchFields.Category = true;

            config.Layout.TransactionWarrantyDisplay.ShowReassuringMessages = true;

            config.Payment.Common.EnableTransactionWarranty = true;

            config.BestSellers.Enabled = false;
            config.Portfolio.Enabled = false;
            config.Subscription.Enabled = false;

            config.CustomFields.Product.Enabled = true;
            config.CustomFields.Seller.Enabled = true;
            config.CustomFields.Buyer.Enabled = true;

            config.Buyer.Enabled = true;
            config.Buyer.UserRatings.Enabled = false;
            config.Seller.UserRatings.Enabled = true;
        }

        private void ConfigureServiceScheduling()
        {
            MaaSConfig config = MaaSConfig.Current;

            config.Product.Enabled = true;
            config.Product.BusinessModel.Rental.Enabled = true;
            config.Product.BusinessModel.Rental.EnablePeriodPerProduct = true;
            config.Product.BusinessModel.Rental.QuantityEnabled = false;
            config.Product.BusinessModel.Rental.RentalPeriodUnit = Domain.StoreModule.Entities.RentalPeriodUnit.Minute;
            config.Product.BusinessModel.Rental.EnablePriceMultiplier = true;
            config.Product.BusinessModel.Rental.HideMinMaxPeriod = true;
            config.Product.BusinessModel.Rental.FixedPeriodRental.Enabled = true;
            config.Product.BusinessModel.Rental.FixedPeriodRental.RegisterByBasePrice = true;
            config.Product.BusinessModel.Rental.ReminderNotification.Enabled = true;
            config.Product.BusinessModel.Rental.GridView.Enabled = true;
            config.Product.BusinessModel.Rental.FormatPriceByRentalPeriodUnit = true;
            config.Product.BusinessModel.Rental.Package.Enabled = true;
            config.Product.BusinessModel.Rental.ProductAvailability.Enabled = true;
            config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled = true;
            config.Product.BusinessModel.Rental.Buyer.Cancel.Enabled = true;
            config.Product.BusinessModel.Rental.Buyer.Schedule.Enabled = true;
            config.Product.BusinessModel.Rental.Buyer.Schedule.MinTime = "06:00";
            config.Product.BusinessModel.Rental.Buyer.Schedule.MaxTime = "22:00";
            config.Product.BusinessModel.Rental.Buyer.ChangePeriod.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.BatchLocks.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.Availability.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.Profile.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.SingleLocks.Enabled = true;
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.MinTime = "06:00";
            config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.MaxTime = "22:00";
            config.Product.BusinessModel.Sale.Enabled = false;

            config.Product.Page.Quantity.Enabled = false;
            config.Product.ActualNoShipping.Enabled = true;
            config.Product.Variants.Enabled = false;
            config.Product.Variants.StockByVariantEnabled = false;
            config.Product.CanPauseProducts = true;
            config.Product.ShowRelatedProducts = true;
            config.Product.Wishlist = true;
            config.Product.Stock.HideCurrentStock = true;

            config.Product.Templates.Enabled = true;
            config.Product.Templates.ForceSellerToUseTemplates = false;

            config.Product.Order.SendOrderCopyToAdmins = true;
            config.Product.Order.ExtraInfoField.Enabled = true;

            config.Search.Enabled = true;
            config.Search.ResultType = Domain.Config.General.ResultType.Product;
            config.Search.SearchType = Domain.Config.General.SearchType.Product;
            config.Search.Map.Enabled = false;
            config.Search.List.Enabled = true;
            config.Search.ShowDescription = false;
            config.Search.SearchFields.Query = true;
            config.Search.SearchFields.Location = true;
            config.Search.SearchFields.OnlyOnSitePickup = false;
            config.Search.SearchFields.Category = true;

            config.BestSellers.Enabled = false;
            config.Portfolio.Enabled = false;
            config.Subscription.Enabled = false;
            config.Quote.Enabled = false;

            config.CustomFields.Product.Enabled = true;
            config.CustomFields.Seller.Enabled = true;
            config.CustomFields.Buyer.Enabled = true;

            config.Layout.TransactionWarrantyDisplay.ShowReassuringMessages = true;

            config.Payment.Common.EnableTransactionWarranty = true;


            config.Buyer.Enabled = true;
            config.Buyer.UserRatings.Enabled = false;
            config.Seller.UserRatings.Enabled = true;
        }

        private void SaveConfig(MaaSConfig config)
        {
            var appConfig = _appConfigRepository.Query().FirstOrDefault();

            if (appConfig == null) appConfig = new ApplicationConfig();

            var oldConfig = appConfig.GetConfig();

            appConfig.MaaSConfig = config.ToJson();

            if (appConfig.Id == 0) _appConfigRepository.Insert(appConfig);
            else _appConfigRepository.Update(appConfig);

            _appConfigRepository.SaveChanges();

            MaaSConfig.Load(appConfig);
            _domainEventDispatcher.Dispatch(new ConfigUpdatedEvent(MaaSConfig.Current, oldConfig));
        }

        public ActionResult PaymentGateway()
        {
            if (!MaaSConfig.Current.Payment.Enabled)
                return HttpNotFound();

            var model = new PaymentGatewayConfigViewModel(MaaSConfig.Current);

            return View("PaymentGateway", model);
        }

        public ActionResult SwitchGateway(string newGateway)
        {
            if (!MaaSConfig.Current.Payment.Enabled)
                return HttpNotFound();

            if (newGateway != "pagarme" && newGateway != "moip")
                return HttpNotFound();

            MaaSConfig.Current.Payment.ActiveGateway = newGateway.ToLower();
            SaveConfig(MaaSConfig.Current);

            return RedirectToAction("PaymentGateway");
        }

        [HttpPost]
        public ActionResult ConfigurePagarMe(PagarmeConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Validate())
                {
                    MaaSConfig.Current.Payment.ActiveGateway = "pagarme";
                    MaaSConfig.Current.Payment.PagarMe.ApiKey = model.ApiKey;
                    MaaSConfig.Current.Payment.PagarMe.EncryptionKey = model.EncryptionKey;
                    MaaSConfig.Current.Payment.PagarMe.StartupRecipient = model.StartupReceiver;
                    SaveConfig(MaaSConfig.Current);

                    _gatewayIdResetter.Run();

                    return RedirectToAction("PaymentGateway");
                }
                else
                {
                    ModelState.AddModelError("Error", "Um ou mais campos contém valores inválidos em produção");
                }
            }

            return PartialView("_PagarMeForm", model);
        }

        [HttpPost]
        public ActionResult ConfigureMoip(MoipConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Validate())
                {
                    MaaSConfig.Current.Payment.ActiveGateway = "moip";

                    MaaSConfig.Current.Payment.Moip.IsSandbox = false;
                    MaaSConfig.Current.Payment.Moip.AccountKey = model.AccountKey;
                    MaaSConfig.Current.Payment.Moip.AccountToken = model.AccountToken;
                    MaaSConfig.Current.Payment.Moip.PublicEncryptionKey = model.PublicEncryptionKey;

                    MaaSConfig.Current.Payment.Moip.AppAccessToken = null;
                    MaaSConfig.Current.Payment.Moip.AppId = null;
                    MaaSConfig.Current.Payment.Moip.AppSecret = null;
                    MaaSConfig.Current.Payment.Moip.NotificationRuleId = null;
                    MaaSConfig.Current.Payment.Moip.StartupReceiverId = null;

                    SaveConfig(MaaSConfig.Current);

                    System.Threading.Thread.Sleep(5000);

                    if (!string.IsNullOrWhiteSpace(MaaSConfig.Current.Payment.Moip.AppId)
                     && !string.IsNullOrWhiteSpace(MaaSConfig.Current.Payment.Moip.AppSecret))
                    {
                        _gatewayIdResetter.Run();
                    }

                    return RedirectToAction("PaymentGateway");
                }
                else
                {
                    ModelState.AddModelError("Error", "Um ou mais campos contém valores inválidos em produção");
                }
            }

            return PartialView("_MoipForm", model);
        }
    }
}