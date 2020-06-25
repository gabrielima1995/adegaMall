using Application.Config;
using Application.Modules.Portfolio;
using Application.Modules.Portfolio.Search;
using Application.Modules.Portfolio.SubCategories;
using Application.Sellers.Moip;
using Application.Sellers.Pagarme;
using Domain.Categories;
using Domain.Core;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Events;
using Domain.Events.PortfolioModule;
using Domain.Events.SellerProfileUpdated;
using Domain.Interfaces;
using Domain.Modules.Finance;
using Domain.Modules.Finance.Interfaces;
using Domain.Modules.Portfolio;
using Domain.Sellers;
using Domain.Services;
using Domain.SubCategories;
using Domain.Utils;
using Framework;
using Geocoding;
using Infra.Crosscutting.Globalization;
using MediatR;
using Microsoft.AspNet.Identity;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Subscriptions;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Moip.Suppliers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.UserAccounts;

namespace Application.Sellers
{
    public class SellerAppService
    {
        private readonly IDomainEventDispatcher dispatcher;

        private readonly ICloudStorage storage;
        private readonly ICustomerOperations customerOperations;
        private readonly IGeocoder geocoder;
        private readonly IGeocodingService geocoding;
        private readonly IRepository<Seller> sellerRepository;
        private readonly IRepository<SocialNetwork> socialNetworkRepository;
        private readonly ISupplierOperations supplierOperations;
        private readonly IRepository<ApplicationUser> userRepository;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly UpdateSellerGatewayCommandHandler updateSellerGatewayHandler;
        private readonly AlternativeNameGenerator alternativeNameGenerator;
        private readonly NicknameGenerator nicknameGenerator;
        private readonly FriendlyUrlGenerator friendlyUrlGenerator;
        private readonly IMediator mediator;
        private readonly CategoryService_Obsolete categoryService;
        private readonly SellerService_Obsolete sellerService;
        private readonly SubCategoryService_Obsolete subCategoryService;
        private readonly IGatewayRegistrationLogRepository registrationLogRepository;

        private readonly PortfolioItemService portfolioItemService;
        private readonly PortfolioService portfolioService;
        private readonly UserManager<ApplicationUser> userManager;

        public SellerAppService(
            IDomainEventDispatcher dispatcher,

            ICloudStorage storage,
            ICustomerOperations customerOperations,
            IGeocoder geocoder,
            IGeocodingService geocoding,
            IRepository<Seller> sellerRepository,
            IRepository<SocialNetwork> socialNetworkRepository,
            ISupplierOperations supplierOperations,
            IRepository<ApplicationUser> userRepository,

            CategoryService_Obsolete categoryService,
            SellerService_Obsolete sellerService,
            SubCategoryService_Obsolete subCategoryService,
            IGatewayRegistrationLogRepository registrationLogRepository,

            PortfolioService portfolioService,
            PortfolioItemService portfolioItemService,
            UserManager<ApplicationUser> userManager,
            ICustomFieldAppService customFieldAppService,
            UpdateSellerGatewayCommandHandler updateSellerGatewayHandler,

            AlternativeNameGenerator alternativeNameGenerator,
            NicknameGenerator nicknameGenerator,
            FriendlyUrlGenerator friendlyUrlGenerator,

            IMediator mediator)
        {
            this.dispatcher = dispatcher;

            this.storage = storage;
            this.customerOperations = customerOperations;
            this.geocoder = geocoder;
            this.geocoding = geocoding;
            this.sellerRepository = sellerRepository;
            this.socialNetworkRepository = socialNetworkRepository;
            this.supplierOperations = supplierOperations;
            this.userRepository = userRepository;

            this.categoryService = categoryService;
            this.sellerService = sellerService;
            this.subCategoryService = subCategoryService;
            this.registrationLogRepository = registrationLogRepository;
            this.portfolioService = portfolioService;
            this.portfolioItemService = portfolioItemService;
            this.userManager = userManager;
            this.customFieldAppService = customFieldAppService;
            this.updateSellerGatewayHandler = updateSellerGatewayHandler;
            this.alternativeNameGenerator = alternativeNameGenerator;
            this.nicknameGenerator = nicknameGenerator;
            this.friendlyUrlGenerator = friendlyUrlGenerator;
            this.mediator = mediator;
        }

        internal Seller GetSellerByEmail(string email)
        {
            return sellerRepository.Get(s => s.Email == email,
                s => s.BankAccount,
                s => s.Address);
        }

        public bool RegisterGateway(string sellerId)
        {
            var model = GetGateway(sellerId);
            return RegisterOrUpdateGateway(model).OK;
        }

        public OperationResult RegisterOrUpdateGateway(SellerGatewayViewModel model)
        {
            var seller = sellerRepository.Get(s => s.Id == model.SellerId,
                s => s.Address,
                s => s.BankAccount);

            if (seller.Address == null)
                return OperationResult.Fail("", GlobalizationHelper.GetString("GATEWAY_REGISTRATION_NEEDS_BASE_ADDRESS"));

            var registered = seller.IntegratorSupplierRegistered && seller.IntegratorCustomerRegistered;
            var registrationResult = registered ? UpdateGateway(seller, model) : RegisterGateway(seller, model);

            if (registrationResult.OK)
            {
                seller.ModifiedAt = DateTime.UtcNow;
                seller.AddDomainEvent(new SellerProfileUpdated(seller.Id));
                sellerRepository.SaveChanges();
            }

            return registrationResult;
        }

        private OperationResult RegisterGateway(Seller seller, SellerGatewayViewModel model)
        {
            if (!MaaSConfig.Current.Payment.Enabled)
                return OperationResult.Success;

            OperationResult validation = null;
            var save = false;

            #region Supplier

            if (model.SellerReceivesPayment && !seller.IntegratorSupplierRegistered)
            {
                IAddress address = null;
                IAddress accountableAddress = null;
                ISupplier supplier;
                IBankAccount bankAccount;
                IAccountable accountable;

                if (model.SellerReceivesMoipPayment)
                {
                    supplier = new MoipSupplier(model.Moip);
                    address = new MoipAddress(model.Moip);
                    bankAccount = new MoipBankAccount(model.Moip);
                    accountable = new MoipAccountable(model.Moip);
                    accountableAddress = new MoipAccountableAddress(model.Moip);
                }
                else if (model.SellerReceivesPagarmePayment)
                {
                    supplier = new PagarmeSupplier(model.Pagarme);
                    bankAccount = new PagarmeBankAccount(model.Pagarme);
                    accountable = new PagarmeAccountable(model.Pagarme);
                }
                else
                {
                    return new OperationResult<Seller>("",
                        "Gateway de pagamento não tratado para o novo modelo de formulário.");
                }

                if (!seller.IntegratorSupplierRegistered)
                {
                    save = true;
                    var supplierAndBankAccount = supplierOperations.Create(supplier, address, bankAccount, accountable, accountableAddress);
                    validation = supplierAndBankAccount;

                    if (validation.OK)
                    {
                        seller.SetGatewayTokens(supplier.GatewayId, supplier.GatewayLiveApiToken, supplier.GatewayTestApiToken, supplier.GatewayUserToken);
                        LogGatewayRegistration(seller.Id, supplierAndBankAccount.Result);
                    }
                }

                if (validation != null && validation.IsFailure)
                    return new OperationResult<Seller>(validation);
            }

            #endregion Supplier

            #region Customer

            if (!seller.IntegratorCustomerRegistered)
            {
                if (string.IsNullOrEmpty(seller.PhoneNumber) && model.SellerReceivesMoipPayment)
                    seller.PhoneNumber = model.Moip.Person.Phone;

                save = true;
                validation = customerOperations.Create(seller, seller.Address);
            }

            if (validation != null && !validation.OK)
                return new OperationResult<Seller>(validation);

            #endregion Customer

            if (save)
            {
                seller.AddDomainEvent(new SellerUpdatedEvent(seller));

                sellerRepository.Update(seller);
                sellerRepository.SaveChanges();
            }

            return OperationResult.Success;
        }

        private OperationResult LogGatewayRegistration(string sellerId, SupplierWithBankAccountResult supplierWithBankAccount)
        {
            GatewayRegistrationLog log = null;

            if (MaaSConfig.Current.Payment.PagarMe.SellerReceivesPagarmePayment)
                log = GatewayRegistrationLog.PagarmeLog(Guid.NewGuid(),
                    sellerId, supplierWithBankAccount.SupplierId, supplierWithBankAccount.BankAccountId, supplierWithBankAccount.ExtraInfoJson);

            if (MaaSConfig.Current.Payment.Moip.SellerReceivesMoipPayment)
                log = GatewayRegistrationLog.WirecardLog(Guid.NewGuid(),
                    sellerId, supplierWithBankAccount.SupplierId, supplierWithBankAccount.BankAccountId, supplierWithBankAccount.ApiToken, supplierWithBankAccount.ExtraInfoJson);

            if (log == null)
                throw new NotImplementedException("Esta operação não foi implementada para o gateway " + MaaSConfig.Current.Payment.ActiveGateway);

            return registrationLogRepository.Add(log);
        }

        private OperationResult UpdateGateway(Seller seller, SellerGatewayViewModel model)
        {
            return updateSellerGatewayHandler.Handle(new UpdateSellerGatewayCommand(seller, model));
        }

        #region Search

        private IQueryable<Seller> SearchFilterBy(SearchRequestViewModel request)
        {
            var subscriptionEnabled = MaaSConfig.Current.Subscription.IsRequired;
            var undergoingValidation = MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var validationMail = MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;
            var paymentActive = MaaSConfig.Current.Payment.Enabled && !MaaSConfig.Current.Search.IgnorePaymentRequirements;

            var sellers = sellerRepository.Query(i => i.Address)
                .Where(i => request.Category == null || i.Portfolio.SubCategories.Any(j => j.Category.Id == request.Category))
                .Where(i => request.SubCategory == null || i.Portfolio.SubCategories.Any(j => j.Id == request.SubCategory))
                .Where(i => request.OnlyAvatar == false || i.Avatar != null)
                .Where(i => request.CityAndState == null || i.Address.CityAndState == request.CityAndState)
                .Where(i => request.AdditionalInfo2 == null || i.AdditionalInfo2 == request.AdditionalInfo2)
                .Where(i => string.IsNullOrEmpty(request.Seller) ||
                    i.Nickname.Contains(request.Seller) ||
                    i.FullName.Contains(request.Seller) ||
                    i.AlternativeName.Contains(request.Seller))
                .Where(i => !subscriptionEnabled ||
                    (undergoingValidation && i.Email != validationMail) ||
                        (i.Subscription != null &&
                            (i.Subscription.Status == SubscriptionStatus.Trialing ||
                                i.Subscription.Status == SubscriptionStatus.Paid ||
                                i.Subscription.Status == SubscriptionStatus.PendingPayment)))
                .Where(i => i.Enabled)
                .Where(i => !paymentActive || !string.IsNullOrEmpty(i.SupplierGatewayId))
                .Where(i => !MaaSConfig.Current.Seller.Moderation.Enabled || i.RequireModeration != true);

            return sellers;
        }

        private void SearchResults(SearchResultViewModel response, SearchRequestViewModel request)
        {
            var queryable = SearchFilterBy(request);
            var isSorted = false;

            #region Sorting

            if (MaaSConfig.Current.Portfolio.Enabled)
            {
                queryable = queryable
                    .OrderByDescending(i => i.Quotes.Count())
                    .ThenBy(i => i.CreatedAt);

                isSorted = true;
            }

            if (MaaSConfig.Current.Product.Enabled)
            {
                queryable = queryable
                    .OrderByDescending(i => i.Rating)
                    .ThenByDescending(i => i.Score)
                    .ThenBy(i => i.CreatedAt);

                isSorted = true;
            }

            if (!isSorted)
            {
                queryable = queryable.OrderBy(x => x.FullName);
                isSorted = true;
            }

            #endregion Sorting

            response.Results = queryable
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToList();
        }

        public SearchViewModel Search(SearchRequestViewModel request)
        {
            var response = new SearchViewModel(request);

            SearchResults(response, request);
            response.Total = SearchFilterBy(request).Count();
            response.Categories = categoryService.SelectorList();
            response.SubCategories = subCategoryService.SelectorList(request.Category);

            return response;
        }

        public SearchResultViewModel SearchOnlyResults(SearchRequestViewModel request)
        {
            var response = new SearchResultViewModel();
            SearchResults(response, request);
            return response;
        }

        #endregion Search

        public async Task<OperationResult> AugumentProfileAsync(SellerUserAndProfile userAndProfile, string externalAvatarUrl)
        {
            var profile = userAndProfile.Profile;

            if (profile.Address != null)
            {
                var geoResult = geocoding.GetCoordinates(profile.Address);
                if (geoResult.OK)
                    profile.Address.SetLocation(geoResult.Result.Lat, geoResult.Result.Lng);
            }

            if (MaaSConfig.Current.Seller.ChangeAvatar && !string.IsNullOrEmpty(externalAvatarUrl))
            {
                var sourceUri = new Uri(externalAvatarUrl);
                var newUri = await storage.UploadFileFromUriAsync(CloudFileType.UserImage.ToString(), Path.GetFileName(sourceUri.LocalPath), sourceUri);
                profile.Avatar = newUri.ToString();
            }

            sellerRepository.SaveChanges();
            return OperationResult.Success;
        }

        public async Task<OperationResult<SellerUserAndProfile>> RegisterAsync(SellerRegisterViewModel request)
        {
            var userAndProfile = request.GetUserAndProfile();
            var registration = await mediator.Send(new RegisterSellerUserAndProfile(userAndProfile, request.Password));

            if (registration.IsFailure)
                return registration.Errors;

            return userAndProfile;
        }

        public SellerProfileViewModel GetProfile(string sellerId)
        {
            var seller = sellerService.Get(i => i.Id == sellerId,
                i => i.Address,
                i => i.BankAccount);
            return new SellerProfileViewModel(seller);
        }

        public SellerGatewayViewModel GetGateway(string sellerId)
        {
            var seller = sellerService.Get(i => i.Id == sellerId, x => x.Address);
            var model = new SellerGatewayViewModel(seller);

            if (model.GatewayRegistered && model.SellerReceivesMoipPayment)
            {
                var op = new MoipSupplierOperations().GetAccountAndBankAccount(seller);
                model.Moip = SellerMoipViewModel.Load(op.Result);
            }
            else if (model.GatewayRegistered && model.SellerReceivesPagarmePayment)
            {
                var op = new PaymentIntegrator.PagarMe.Suppliers.SupplierOperations().GetBankAccount(seller);
                model.Pagarme = SellerPagarmeViewModel.Load(op.Result);
            }

            if (seller.Address != null)
            {
                model.SellerAddress = seller.Address;
            }

            return model;
        }

        public OperationResult<Seller> Update(SellerProfileViewModel model, string sellerUserId)
        {
            if (userRepository.Query().Any(user => user.UserName == model.Email && user.Id != sellerUserId))
                return new OperationResult<Seller>(
                    key: nameof(model.Email),
                    errors: Thread.CurrentThread.GetStringLocalized("EMAIL_ALREADY_EXISTS_ERROR_MESSAGE"));

            if (sellerService.Exists(_seller => _seller.Nickname == model.Nickname && _seller.Id != model.Id))
                return new OperationResult<Seller>(
                    key: nameof(model.Nickname),
                    errors: Thread.CurrentThread.GetStringLocalized("NICKNAME_ALREADY_EXISTS_ERROR_MESSAGE"));

            if (!DocumentNumberValidator.IsValidForSeller(model.DocumentNumber))
                return new OperationResult<Seller>(
                    key: nameof(model.DocumentNumber),
                    errors: Thread.CurrentThread.FormatStringLocalized("NOT_VALID_FIELD_ERROR", Thread.CurrentThread.GetStringLocalized("DOCUMENT_NUMBER_LABEL")));

            if (MaaSConfig.Current.CustomFields.Seller.Enabled)
                if (model.HasCustomFields())
                    customFieldAppService.StoreCustomFields(model);

            var seller = sellerService.Update(
                entity => entity.Id == model.Id,
                entity =>
                {
                    entity.AddDomainEvent(new SellerUpdatedEvent(entity));
                    entity.AddDomainEvent(new SellerProfileUpdated(entity.Id));
                    model.Update(entity, storage, geocoder, friendlyUrlGenerator);
                },
                entity => entity.Address,
                entity => entity.BankAccount);
            return seller;
        }

        #region Subscription

        public void StartSubscription(string sellerId)
        {
            sellerService.Update(
                get: seller => seller.Id == sellerId,
                update: seller =>
                {
                    seller.AddDomainEvent(new SellerUpdatedEvent(seller));
                    seller.SubscriptionStarted = true;
                });
        }

        #endregion Subscription

        #region Portfolio

        private PortfolioViewModel GetPortfolioInternal(IList<SubCategory> subCategories, IList<SellerSocialMedia> socialMedia, IList<PortfolioItem> items)
        {
            var response = new PortfolioViewModel();

            subCategories = subCategories ?? new List<SubCategory>();
            socialMedia = socialMedia ?? new List<SellerSocialMedia>();
            items = items ?? new List<PortfolioItem>();

            var categoriesList = categoryService.SelectorList();
            var subCategorysList = subCategoryService.SelectorList();
            var socialNetworksList = socialNetworkRepository.Query().ToList();

            #region Sub Categories

            if (subCategories.Count < 1)
            {
                subCategories.Add(new SubCategory
                {
                    Category = new Category()
                });
            }

            foreach (var subCategory in subCategories)
            {
                response.SubCategories.Add(new SubCategoryAndCategorySelector
                {
                    SubCategory = subCategory.Id,
                    Category = subCategory.Category.Id,
                    SubCategories = subCategorysList
                        .Where(i => i.Category.Id == subCategory.Category.Id)
                        .ToList(),
                    Categories = categoriesList
                });
            }

            #endregion Sub Categories

            #region Social Networks

            foreach (var socialNetwork in socialNetworksList)
            {
                response.SocialMedia.Add(new PortfolioSocialMediaViewModel
                {
                    SocialNetwork = socialNetwork,
                    SellerSocialMedia = socialMedia
                        .FirstOrDefault(i => i.SocialNetwork.Id == socialNetwork.Id)
                });
            }

            #endregion Social Networks

            #region Portfolio

            response.Items = items.Select(x => new PortfolioItemVM(x)).ToList();
            if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
            {
                foreach (var item in response.Items)
                {
                    customFieldAppService.ReadCustomFieldsToViewModel(item);
                }
            }

            #endregion Portfolio

            return response;
        }

        public PortfolioViewModel GetPortfolio(string id)
        {
            var seller = sellerRepository.Get(i => i.Id == id,
                i => i.Portfolio,
                i => i.Portfolio.Items,
                i => i.Portfolio.Items.Select(j => j.Images),
                i => i.Portfolio.SubCategories,
                i => i.Portfolio.SubCategories.Select(j => j.Category),
                i => i.Portfolio.SocialMedia,
                i => i.Portfolio.SocialMedia.Select(j => j.SocialNetwork));

            return GetPortfolioInternal(seller.Portfolio?.SubCategories, seller.Portfolio?.SocialMedia, seller.Portfolio?.Items?.Where(x => !x.Deleted).ToList());
        }

        public PortfolioViewModel GetPortfolio(string id, IList<SubCategoryAndCategorySelector> subCategory)
        {
            var seller = sellerRepository.Get(i => i.Id == id,
                i => i.Portfolio,
                i => i.Portfolio.Items,
                i => i.Portfolio.Items.Select(j => j.Images),
                i => i.Portfolio.SocialMedia,
                i => i.Portfolio.SocialMedia.Select(j => j.SocialNetwork));

            var subCategorysList = subCategory
                .Select(i => new SubCategory
                {
                    Id = i.SubCategory ?? 0,
                    Category = new Category
                    {
                        Id = i.Category ?? 0
                    }
                })
                .ToList();

            return GetPortfolioInternal(subCategorysList, seller.Portfolio?.SocialMedia, seller.Portfolio?.Items?.Where(x=> !x.Deleted).ToList());
        }

        public void SavePortfolio(PortfolioRequestViewModel request)
        {
            var subCategoryIds = request.SubCategories
                .Select(i => i.SubCategory)
                .Distinct()
                .ToList();

            var subCategories = subCategoryService
                .Query()
                .Where(i => subCategoryIds.Any(j => j == i.Id))
                .ToList();

            var seller = sellerRepository.Get(s => s.Id == request.SellerId);
            seller.ModifiedAt = DateTime.UtcNow;
            sellerRepository.SaveChanges();

            portfolioService.Save(i => i.SellerId == request.SellerId,
                () =>
                {
                    var portfolio = request.Create(subCategories);
                    portfolio.AddDomainEvent(new PortfolioCreatedEvent(portfolio));
                    portfolio.AddDomainEvent(new SellerProfileUpdated(seller.Id));
                    return portfolio;
                },
                item =>
                {
                    request.Update(item, subCategories);
                    item.AddDomainEvent(new PortfolioUpdatedEvent(item));
                    item.AddDomainEvent(new SellerProfileUpdated(seller.Id));
                },
                i => i.SubCategories);
        }

        public PortfolioItemViewModel PrepareNewPortfolioItem(string sellerId)
        {
            return new PortfolioItemViewModel(sellerId) { Id = Guid.NewGuid() };
        }

        public PortfolioItemViewModel GetPortfolioItem(Guid portfolioItemId, string sellerId)
        {
            var entity = portfolioItemService.Get(i => i.Id == portfolioItemId && i.Portfolio.SellerId == sellerId,
                i => i.Portfolio,
                i => i.Portfolio.Items,
                i => i.SubCategories,
                i => i.SubCategories.Select(j => j.Category),
                i => i.Images);

            return new PortfolioItemViewModel(entity);
        }

        public OperationResult<PortfolioItem> SavePortfolioItem(PortfolioItemViewModel model)
        {
            var subCategoryIds = model.SubCategories
                .Select(i => i.SubCategory)
                .Distinct()
                .ToList();

            var subCategories = subCategoryService
                .Query()
                .Where(i => subCategoryIds.Any(j => j == i.Id))
                .ToList();

            var seller = sellerService.Get(i => i.Id == model.SellerId,
                i => i.Portfolio,
                i => i.Portfolio.Items);

            var config = MaaSConfig.Current;
            var portfolio = seller.Portfolio;
            var errors = model.Validate();

            if (portfolio == null)
                portfolio = new Portfolio { Seller = seller };
            else if (model.Id == default(Guid) &&
                config.Portfolio.ItemsLimit != 0 &&
                config.Portfolio.ItemsLimit <= portfolio.Items.Count)
                errors.Add("", CultureInfo.CurrentCulture.GetStringLocalized("PORTFOLIO_ITEM_LIMIT_MESSAGE"));

            if (!errors.HasErrors)
                return portfolioItemService.Save(
                    get: i => i.Id == model.Id && i.Portfolio.SellerId == model.SellerId,
                    create: () =>
                    {
                        var item = model.Create(portfolio, subCategories, storage, friendlyUrlGenerator);
                        item.AddDomainEvent(new PortfolioItemAddedEvent(model.SellerId, item));
                        item.AddDomainEvent(new SellerProfileUpdated(seller.Id));
                        return item;
                    },
                    update: entity =>
                    {
                        model.Update(entity, subCategories, storage, friendlyUrlGenerator);
                        entity.AddDomainEvent(new PortfolioItemUpdatedEvent(model.SellerId, entity));
                        entity.AddDomainEvent(new SellerProfileUpdated(seller.Id));
                    },
                    includes: new Expression<Func<PortfolioItem, object>>[]
                    {
                        i => i.Images,
                        i => i.SubCategories,
                        i => i.SubCategories.Select(j => j.Category)
                    });

            seller.ModifiedAt = DateTime.UtcNow;
            sellerRepository.SaveChanges();

            return errors;
        }



        public void DeletePortfolioItem(Guid portfolioItemId, string sellerId)
        {
            var item = portfolioItemService.DeletePortfolioItem(portfolioItemId, sellerId);

            if (item.Result != null || !item.IsFailure)
            {
                sellerRepository.SaveChanges();
                dispatcher.Dispatch(new PortfolioItemRemovedEvent(sellerId, item.Result));
                dispatcher.Dispatch(new SellerProfileUpdated(sellerId));
            }
        }

        #endregion Portfolio
    }

    public class Accountable : IAccountable
    {
        public Accountable()
        {
        }

        public Accountable(Seller seller)
        {
            Name = seller.AccountableName ?? seller.Name;
            DocumentNumber = seller.AccountableDocumentNumber ?? seller.DocumentNumber;
            PhoneNumber = seller.Phone;
        }

        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public string PhoneNumber { get; set; }
    }
}