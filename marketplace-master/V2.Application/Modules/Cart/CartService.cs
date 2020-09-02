using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.QuoteBillingModule.Interfaces;
using Domain.Modules.StoreModule;
using Domain.Modules.StoreModule.Coupons;
using Domain.Modules.StoreModule.Entities;
using Domain.Services.StoreModule;
using Domain.Validation;
using Framework;
using Infra.Crosscutting.Globalization;
using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using V2.Application.Modules.General.CustomFields;

namespace V2.Application.Modules
{
    public class CartService
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        private readonly RentalService rentalService;
        private readonly ProductAvailabilityService _productAvailabilityService;
        private readonly ITimeZoneProvider timeZoneProvider;
        private readonly ICartRepository cartRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IProductRepository productRepository;
        private readonly IQuoteBillingRepository quoteBillingRepository;
        private readonly IShippingService shippingService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly ICustomFieldRepository customFieldRepository;
        private readonly ICouponRepository couponRepository;

        public CartService(
            RentalService rentalService,
            ProductAvailabilityService productAvailabilityService,
            ITimeZoneProvider timeZoneProvider,
            ICartRepository cartRepository,
            IBuyerRepository buyerRepository,
            IProductRepository productRepository,
            IQuoteBillingRepository quoteBillingRepository,
            IShippingService shippingService,
            IUnitOfWork uow,
            ICustomFieldAppService customFieldAppService,
            ICustomFieldRepository customFieldRepository,
            ICouponRepository couponRepository)
        {
            this.rentalService = rentalService;
            _productAvailabilityService = productAvailabilityService;
            this.timeZoneProvider = timeZoneProvider;
            this.cartRepository = cartRepository;
            this.buyerRepository = buyerRepository;
            this.productRepository = productRepository;
            this.quoteBillingRepository = quoteBillingRepository;
            this.shippingService = shippingService;
            unitOfWork = uow;
            this.customFieldAppService = customFieldAppService;
            this.customFieldRepository = customFieldRepository;
            this.couponRepository = couponRepository;
        }

        public OperationResult<Cart> SetExtraInfo(string buyerId, string extraInfo)
        {
            var cart = Get(buyerId);
            cart.ExtraInfo = extraInfo;

            var result = cartRepository.Update(cart, true);
            if (result.IsFailure)
                return result.Errors;

            return cart;
        }

        public OperationResult<Cart> AddItemWithoutShipping(string buyerId, Guid productId, int quantity, DateTime? rentalStartUtc, DateTime? rentalEndUtc, int multiplier, int? variant, Guid? productAvailabilityId, CartCustomFieldsViewModel cartCustomFieldView = null, bool isQuote = false)
        {
            if (rentalStartUtc != null && rentalStartUtc.Value.Kind != DateTimeKind.Utc)
                throw new InvalidOperationException($"Rental start date must come in UTC.");

            if (rentalEndUtc != null && rentalEndUtc.Value.Kind != DateTimeKind.Utc)
                throw new InvalidOperationException($"Rental end date must come in UTC.");

            var cart = Get(buyerId);
            var product = productRepository.GetForCartOperations(productId);

            #region Rental

            if (config.Product.BusinessModel.Rental.Enabled && !isQuote)
            {
                if (product.RentalSettings.IsFixedRental && rentalStartUtc.HasValue)
                {
                    rentalEndUtc = product.RentalSettings.CalculateEndDateTime(rentalStartUtc);
                }

                var rentalItem = cart.GetItemByProduct(product, rentalStartUtc, rentalEndUtc);

                if (rentalItem != null && rentalItem.RentalInfo.StartDate == rentalStartUtc && rentalItem.RentalInfo.EndDate == rentalEndUtc)
                {
                    var rentalNewQty = (int)((rentalItem?.Quantity ?? 0M) + quantity);
                    return ChangeQuantityInternal(cart, product, rentalNewQty, rentalStartUtc, rentalEndUtc, multiplier, productAvailabilityId: productAvailabilityId, cartCustomFieldViewModel: cartCustomFieldView);
                }
                else
                {
                    var rentalNewQty = quantity;

                    if (cart.CheckRentalOverlap(product, rentalStartUtc, rentalEndUtc))
                        return new OperationResult<Cart>("Error", "Período já selecionado");

                    return ChangeQuantityInternal(cart, product, rentalNewQty, rentalStartUtc, rentalEndUtc, multiplier, productAvailabilityId: productAvailabilityId, cartCustomFieldViewModel: cartCustomFieldView);
                }
            }

            #endregion Rental

            if (cart.Items.Count == 0 && !product.SupportsShippingMethod(cart.ShippingMethod))
            {
                var commonShippingMethods = product.GetShippingMethods().Intersect(config.Product.GetShippingMethodsEnabled());
                if (commonShippingMethods.Any()) cart.ShippingMethod = commonShippingMethods.First();
            }

            var item = cart.GetItemByProduct(product, variant: variant);
            var newQuantity = (int)((item?.Quantity ?? 0M) + quantity);
            var changeQuantityResult = ChangeQuantityInternal(cart, product, newQuantity, rentalStartUtc, rentalEndUtc, multiplier, variant, productAvailabilityId, cartCustomFieldViewModel: cartCustomFieldView, isQuote: isQuote);

            return changeQuantityResult;
        }

        public OperationResult<Cart> ChangeQuantityWithoutShipping(string buyerId, Guid productId, int newQuantity, int? variant, Guid? productAvailabilityId)
        {
            var cart = Get(buyerId);
            var product = productRepository.GetForCartOperations(productId);
            return ChangeQuantityInternal(cart, product, newQuantity, variant: variant, productAvailabilityId: productAvailabilityId);
        }

        private OperationResult<CorreiosShippingCollection> EnsureStockAndShipping(Cart cart)
        {
            var errors = new ErrorDictionary();
            var shippingCollection = CorreiosShippingCollection.Empty();
            foreach (var item in cart.Items)
            {
                var stockValidation = item.Product.CheckStock(item.Quantity, item.Variant?.Id);
                if (stockValidation.IsFailure) errors.Merge(stockValidation.Errors);

                var shippingResult = GetItemShipping(cart, new ProductWithQty(item.Product, item.Quantity, item.RentalInfo.StartDate));
                if (shippingResult.IsFailure) errors.Merge(shippingResult.Errors);
                else
                {
                    shippingCollection = shippingCollection.Merge(shippingResult.Result);
                    item.Shipping = shippingResult.Result.TotalShippingPrice;
                    if (cart.ShippingMethod == ShippingMethod.PostalRegular || cart.ShippingMethod == ShippingMethod.PostalExpress || cart.ShippingMethod == ShippingMethod.AsapLog || cart.ShippingMethod == ShippingMethod.TotalExpress)
                        item.DeliveryDays = shippingResult.Result.TotalDeliveryDays;
                }

                cartRepository.Update(item);
            }

            var updateResult = UpdateCartShipping(cart);
            if (updateResult.Errors.HasErrors) errors.Merge(updateResult.Errors);

            var validateCouponResult = ValidateCoupon(cart);
            if (validateCouponResult.IsFailure) errors.Merge(validateCouponResult.Errors);

            var commitResult = unitOfWork.Save();
            if (!commitResult.OK) errors.Merge(commitResult.Errors);

            if ((commitResult.IsFailure && updateResult.IsFailure) || errors.HasErrors)
                return errors;
            return new OperationResult<CorreiosShippingCollection>(shippingCollection);
        }

        private OperationResult ValidateCoupon(Cart cart)
        {
            void RemoveDiscount()
            {
                cart.Discount = decimal.Zero;
            }

            OperationResult InvalidCoupon()
            {
                RemoveDiscount();

                return OperationResult.Fail("INVALID_COUPON", "Cupom inválido.");
            }

            if (cart.IsEmpty || cart.Coupon == null)
            {
                RemoveDiscount();

                return OperationResult.Success;
            }

            var coupon = cart.Coupon;

            void CalculateDiscount()
            {
                // somente produtos sem desconto
                var items = cart.Items
                    .Where(i =>
                        i.Discount == 0 &&
                        !i.Product.ProductPriceMapped.DisplayPrice.Discount.HasDiscount);

                // cupom do vendedor, somente produtos dele
                if (coupon.IsBySeller)
                {
                    items = items.Where(i => i.Product.Store_SellerId == coupon.SellerId);
                }

                var amount = items.Sum(i => i.SubTotal);

                var discount = coupon.CalculateDiscount(amount);

                cart.Discount = Math.Min(discount, amount);
            }

            if (!coupon.IsValid()) return InvalidCoupon();

            switch (coupon.UseMode)
            {
                // primeira compra
                case CouponUseMode.FirstOrder:
                    var firstOrder = couponRepository.IsFirstOrder(cart.Buyer.Id);
                    if (!firstOrder) return InvalidCoupon();
                    break;

                // Uso único por usuário
                case CouponUseMode.OneTimeByUser:
                    var oneTimeByUser = couponRepository.IsOneTimeByUser(cart.Buyer.Id, coupon.Id);
                    if (!oneTimeByUser) return InvalidCoupon();
                    break;

                // Uso único no marketplace
                case CouponUseMode.OneTimeEver:
                    var oneTimeEver = couponRepository.IsOneTimeEver(coupon.Id);
                    if (!oneTimeEver) return InvalidCoupon();
                    break;
            }

            if (coupon.IsDiscount)
                CalculateDiscount();

            cartRepository.Update(cart);

            return OperationResult.Success;
        }

        public Cart Get(string buyerId)
        {
            var cart = GetOrNew(buyerId, config.Product.ShippingMode);

            if (cart.ShippingMode != config.Product.ShippingMode)
                SetShippingMode(cart, config.Product.ShippingMode);

            var availableShippingMethods = config.Product.GetShippingMethodsEnabled();
            var availableShippingMethodsProduct = cart.GetAvailableShippingMethods();
            var cartShippingModeIsAvailable = availableShippingMethods.Any(method => method == cart.ShippingMethod);
            if (!cartShippingModeIsAvailable)
                cart.ShippingMethod = availableShippingMethods.FirstOrDefault();

            cart.AvailableShippingsMethods = GetAvailableShippingMethods(cart, availableShippingMethodsProduct);
            cart.ShippingMethod = SetAvailableShippingMethodOrDefault(cart);
            cart.StockAndShippingValidation = EnsureStockAndShipping(cart);
            return cart;
        }

        private ShippingMethod SetAvailableShippingMethodOrDefault(Cart cart)
        {
            if (cart.AvailableShippingsMethods.Contains(cart.ShippingMethod))
                return cart.ShippingMethod;
            else if (cart.AvailableShippingsMethods.Any())
                return cart.AvailableShippingsMethods.First();
            else return cart.ShippingMethod;
        }

        private List<ShippingMethod> GetAvailableShippingMethods(Cart cart, List<ShippingMethod> shippingMethods)
        {
            List<ShippingMethod> shippingsForCart = new List<ShippingMethod>();
            var selectedShipping = cart.ShippingMethod;
            foreach (var item in cart.Items)
            {
                foreach (var shippingMethod in shippingMethods)
                {
                    cart.ShippingMethod = shippingMethod;
                    var shippingResult = GetItemShipping(cart, new ProductWithQty(item.Product, item.Quantity, item.RentalInfo.StartDate));

                    if (!shippingResult.Errors.Any() && !shippingResult.Result.Any(x => x.ResultType == Domain.StoreModule.Entities.CorreiosResultType.Error))
                    {
                        if (!shippingsForCart.Contains(shippingMethod))
                            shippingsForCart.Add(shippingMethod);
                    }
                }
            }
            cart.ShippingMethod = selectedShipping;
            return shippingsForCart;
        }

        public Checkout GetCheckout(string buyerId)
        {
            var cart = Get(buyerId);

            SetValidShippingMethod(cart);
            var cartValidation = EnsureStockAndShipping(cart).Errors;

            if (!cartValidation.HasErrors)
            {
                var validationResult = cart.IsValidForCheckout();
                cartValidation.Merge(validationResult.Errors);
            }

            var buyer = buyerRepository.GetWithAddresses(buyerId);
            var model = new Checkout(cart, cartValidation);

            var buyerValidationResult = new BuyerCheckoutValidation().Validate(buyer);
            if (!buyerValidationResult.IsValid) model.CartValidation.Add(buyerValidationResult);

            SetValidPaymentMethod(model);
            model.Payment.InstallmentOptions = ListInstallmentOptions(cart.Amount, cart);

            return model;
        }

        public bool ProductWouldCauseDoubleFreight(string buyerId, Guid productId)
        {
            /*
             Se o modelo de frete do marketplace for unificado por vendedor
             e o comprador colocar produto de um vendedor diferente no carrinho,
             pagará um segundo frete. Esta função verifica se ele deve ser avisado disso.
             */

            if (config.Product.ShippingMode != ShippingMode.PerStore)
                return false;

            var cart = Get(buyerId);

            if (cart.Items.Count == 0)
                return false;

            if (cart.Items.Select(x => x.Product.Store.SellerId).Distinct().Count() > 1)
                return false;

            var product = productRepository.GetForCartOperations(productId);

            if (cart.Items.Select(x => x.Product.Store.SellerId).First() == product.Store.SellerId)
                return false;

            return true;
        }

        public void RemoveAllItems(string buyerId)
        {
            var cart = Get(buyerId);

            foreach (var item in cart.Items.ToArray())
            {
                var product = productRepository.GetForCartOperations(item.Product.Id);
                ChangeQuantityInternal(cart, product, 0);
            }
        }

        public OperationResult<Cart> RemoveItemWithoutShipping(string buyerId, Guid productId, int? variant, Guid? productAvailabilityId)
        {
            var cart = Get(buyerId);
            var product = productRepository.GetForCartOperations(productId);
            return ChangeQuantityInternal(cart, product, 0, variant: variant, productAvailabilityId: productAvailabilityId, isRemoving: true);
        }

        public OperationResult SetShippingAddress(string buyerId, Guid addressId)
        {
            var cart = Get(buyerId);
            var buyer = buyerRepository.GetWithAddresses(buyerId);

            var allAddresses = buyer.GetAllAddresses();
            var mainAddress = allAddresses.FirstOrDefault(i => i.Id == addressId) ?? allAddresses.FirstOrDefault();

            cart.ShippingAddress = mainAddress;

            var updateCartResult = cartRepository.Update(cart, saveChanges: true);
            if (!updateCartResult.OK) return updateCartResult.Errors;

            var ensureResult = EnsureStockAndShipping(cart);
            if (!ensureResult.OK) return ensureResult.Errors;

            return OperationResult.Success;
        }

        public OperationResult SetShippingMethod(string buyerId, ShippingMethod shippingMethod)
        {
            if (!config.Product.GetShippingMethodsEnabled().Any(i => i == shippingMethod))
                return new OperationResult("", $"Shipping method {shippingMethod} is not enabled.");

            var cart = Get(buyerId);
            cart.ShippingMethod = shippingMethod;

            var updateCartResult = cartRepository.Update(cart, saveChanges: true);
            if (!updateCartResult.OK) return updateCartResult.Errors;

            var ensureResult = EnsureStockAndShipping(cart);
            if (!ensureResult.OK) return ensureResult.Errors;

            return OperationResult.Success;
        }

        public OperationResult SetCoupon(string buyerId, string couponCode)
        {
            Coupon coupon = null;

            if (!string.IsNullOrEmpty(couponCode))
            {
                coupon = couponRepository.GetByCode(couponCode);
            }

            var cart = Get(buyerId);
            cart.Coupon = coupon;

            if (cart.StockAndShippingValidation.IsFailure)
            {
                cart.StockAndShippingValidation.Errors.RemoveAll(i => i.Key == "INVALID_COUPON");
            }

            var updateCartResult = cartRepository.Update(cart, saveChanges: true);
            if (!updateCartResult.OK) return updateCartResult.Errors;

            var ensureResult = EnsureStockAndShipping(cart);
            if (!ensureResult.OK) return ensureResult.Errors;

            return OperationResult.Success;
        }

        public OperationResult SetShippingMode(Cart cart, ShippingMode shippingMode)
        {
            cart.ShippingMode = shippingMode;
            return cartRepository.Update(cart, saveChanges: true);
        }

        private OperationResult<Cart> ChangeQuantityInternal(Cart cart, Product product, int newQuantity, DateTime? rentalStartUtc = null, DateTime? rentalEndUtc = null, int multiplier = 0, int? variant = null, Guid? productAvailabilityId = null, bool isRemoving = false, IWithCustomFields cartCustomFieldViewModel = null, bool isQuote = false)
        {
            var isNew = false;

            if (!(config.Product.Variants.StockByVariantEnabled && product.StockByVariant))
                variant = null;

            if (!isRemoving && config.Product.BusinessModel.Rental.EnablePriceMultiplier &&
                product.MinMultiplier.GetValueOrDefault() > 0 &&
                product.MaxMultiplier.GetValueOrDefault() > 0 &&
                product.MultiplierPrice.GetValueOrDefault() > 0 &&
                (multiplier < product.MinMultiplier.Value || multiplier > product.MaxMultiplier.Value))
                return new ErrorDictionary("", GlobalizationHelper.GetString("CART_INVALID_MULTIPLIER_VALUES"));

            var item = cart.GetItemByProduct(product, variant: variant);

            // Same items with different rental dates should be considered different items
            if (item != null && item.RentalInfo != null && rentalStartUtc.HasValue && rentalEndUtc.HasValue
                && rentalStartUtc != item.RentalInfo.StartDate && rentalEndUtc != item.RentalInfo.EndDate)
                item = null;

            if (item == null)
            {
                if (newQuantity <= 0) return cart;

                if (config.Product.BusinessModel.Rental.Enabled && !isQuote)
                    if ((!rentalStartUtc.HasValue || !rentalEndUtc.HasValue) && (product.ProductType == ProductType.Default || productAvailabilityId.HasValue))
                        return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_RENTAL_NULL_DATES_ERROR_MESSAGE"));

                item = new CartItem(cart, product, 0, 0M);

                if (config.Product.BusinessModel.Rental.EnablePriceMultiplier)
                    item.Multiplier = multiplier;

                if (variant != null)
                {
                    item.Variant = product.StockVariants.FirstOrDefault(i => i.Variant.Id == variant)?.Variant;
                    item.ByVariant = item.Variant != null;
                }

                cart.Items.Add(item);
                cartRepository.Add(item);

                isNew = true;
            }
            else
            {
                rentalStartUtc = item.RentalInfo.StartDate;
                rentalEndUtc = item.RentalInfo.EndDate;
            }

            if (newQuantity <= 0)
            {
                cart.Items.Remove(item);
                cartRepository.Delete(item);
                customFieldRepository.DeleteForEntityId(item.Id);
            }
            else
            {
                var stockValidation = product.CheckStock(newQuantity, variant);
                if (!stockValidation.OK) return stockValidation.Errors;

                item.Quantity = newQuantity;

                if (config.Product.BusinessModel.Rental.Enabled && !isQuote)
                {
                    if (product.ProductType == ProductType.Default || productAvailabilityId.HasValue)
                    {
                        var timeZone = timeZoneProvider.GetTimeZone();
                        var rentalValidation = ChangeRentalProduct(item, rentalStartUtc.Value, rentalEndUtc.Value, timeZone, productAvailabilityId);
                        if (!rentalValidation.OK) return rentalValidation.Errors;
                    }
                    else
                    {
                        var rentalValidation = ChangeRentalProduct(item, product.RentalSettings.MinRentalPeriod.Value);
                        if (!rentalValidation.OK) return rentalValidation.Errors;
                    }
                }

                if (!isNew) cartRepository.Update(item);
            }

            if (config.Quote.Enabled && config.Quote.QuoteBilling.Enabled && isQuote)
            {
                var productIds = cart.Items.Select(x => x.Product.Id).ToList();

                if (quoteBillingRepository.ListContainsBillingProducts(productIds))
                    return new ErrorDictionary("", GlobalizationHelper.GetString("CART_CANNOT_MIX_BILLING_WITH_COMMON_PRODUCTS"));
            }
            if (cartCustomFieldViewModel != null && cartCustomFieldViewModel.HasCustomFields())
            {
                cartCustomFieldViewModel.Id = item.Id;
                customFieldAppService.StoreCustomFields(cartCustomFieldViewModel);
            }

            var validateCouponResult = ValidateCoupon(cart);
            if (validateCouponResult.IsFailure) return validateCouponResult.Errors;

            var commitResult = unitOfWork.Save();
            if (!commitResult.OK) return commitResult.Errors;

            return cart;
        }

        private OperationResult ChangeRentalProduct(CartItem item, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone, Guid? productAvailabilityId)
        {
            var validation = rentalService.CheckAvailability(item.Product, item.Quantity, rentalStartUtc, rentalEndUtc, timeZone,
                productAvailabilityId: productAvailabilityId);

            if (!validation.OK) return validation;

            ProductAvailability productAvailability = null;

            if (config.Product.BusinessModel.Rental.ProductAvailability.Enabled &&
                productAvailabilityId.HasValue)
            {
                productAvailability = _productAvailabilityService.GetById(
                    productAvailabilityId.Value,
                    item.Product.Store_SellerId);

                if (productAvailability == null) return OperationResult.Fail("Product availability invalid.");
            }

            item.Rental(rentalStartUtc, rentalEndUtc,
                item.Product.RentalSettings.RentalPeriodUnit.Value, productAvailability);

            return OperationResult.Success;
        }

        private OperationResult ChangeRentalProduct(CartItem item, int fixedPeriod)
        {
            item.Rental(fixedPeriod, item.Product.RentalSettings.RentalPeriodUnit.Value);

            return OperationResult.Success;
        }

        private void ConsolidateCartSellers(Cart cart)
        {
            foreach (var seller in cart.Sellers.ToList())
                cartRepository.Delete(seller);

            var sellers = cart.Items.Select(x => x.Product.Store.Seller).ToList();

            foreach (var seller in sellers)
            {
                if (!cart.Sellers.Any(x => x.Seller.Id == seller.Id))
                {
                    var cartSeller = new CartSeller(cart, seller, 0M);
                    cart.Sellers.Add(cartSeller);
                    cartRepository.Add(cartSeller);
                }
            }
        }

        private OperationResult<CorreiosShippingCollection> GetItemShipping(Cart cart, ProductWithQty product)
        {
            switch (cart.ShippingMode)
            {
                case ShippingMode.PerProduct:
                    {
                        var adapter = shippingService.GetAdapter(cart.ShippingMethod);

                        var shippingResult = adapter.GetShippingForProduct(
                            product,
                            product.Product.Store.Seller.Address,
                            cart.ShippingAddress,
                            config.Product.BusinessModel.Rental.Enabled && (product.Product.RentalSettings.ChargeDoubleShipping ?? false));

                        if (shippingResult.OK)
                            return shippingResult.Result;

                        return shippingResult.Errors;
                    }
                case ShippingMode.PerStore:
                    return CorreiosShippingCollection.Empty();

                default:
                    throw new NotImplementedException();
            }
        }

        private Cart GetOrNew(string buyerId, ShippingMode shippingMode)
        {
            bool isNew = false;
            var cart = cartRepository.GetFull(buyerId);

            if (cart == null)
            {
                var buyer = buyerRepository.GetWithAddresses(buyerId);

                isNew = true;
                cart = new Cart { Buyer = buyer };
                cartRepository.Add(cart);
            }
            else
            {
                // TODO: verificar se modelo de assinatura está ativo e, nesse caso, se o vendedor tem assinatura ativa.
                cart.Items = cart.Items.Where(i => !i.Product.Deleted).ToList();
            }

            cart.ShippingMode = shippingMode;

            if (cart.ShippingAddress == null)
                cart.ShippingAddress = cart.Buyer.GetAllAddresses().FirstOrDefault();

            if (!isNew)
                cartRepository.Update(cart);

            var commitResult = unitOfWork.Save();
            if (!commitResult.OK) throw new Exception(commitResult.Errors.ToSimpleString());

            return cart;
        }

        private List<InstallmentOption> ListInstallmentOptions(decimal orderAmount, Cart cart)
        {
            var config = MaaSConfig.Current;

            int maxInstallments = config.Payment.PaymentGateway.Installments.MaxInstallments;

            if (config.Product.EnableInstallmentsPerProduct && cart.Items.Count > 0)
            {
                // Selects the product with smallest Installment limit under the global limit
                maxInstallments = Math.Min(cart.Items.OrderBy(x => x.Product.MaxInstallments).First().Product.MaxInstallments.Value, config.Payment.PaymentGateway.Installments.MaxInstallments);
            }

            if (config.Payment.PaymentGateway.CreditCard.Enabled == false ||
                config.Payment.PaymentGateway.Installments.MaxInstallments == 1 || maxInstallments == 1 ||
                orderAmount < config.Payment.PaymentGateway.Installments.MinOrderAmount)
            {
                return new List<InstallmentOption>(1)
                {
                    new InstallmentOption
                    {
                        InstallmentsCount = 1,
                        BaseAmount = orderAmount,
                        InterestAmount = 0M
                    }
                };
            }

            var options = new List<InstallmentOption>();

            for (var numberOfInstallments = 1; numberOfInstallments <= maxInstallments; numberOfInstallments++)
            {
                var interestAmount = 0M;

                if (config.Payment.GatewayIsWirecard() && config.Payment.Moip.Options.CustomerPaysFees)
                {
                    var anticipationFee = config.Payment.Moip.Options.FeesConfig.AnticipationFees[numberOfInstallments] / 100m;
                    var transactionFee = CalculateInstallmentFeesIfHasConfig(numberOfInstallments);

                    // link sobre o cálculo dos installmentsFees e anticipationFee https://wirecardbrasil.github.io/interest-calculator/
                    interestAmount = orderAmount * ((1 - transactionFee) / (1 - transactionFee - anticipationFee)) - orderAmount;
                    interestAmount = (interestAmount / numberOfInstallments).RoundTo2Places();
                }

                var opt = new InstallmentOption
                {
                    InstallmentsCount = numberOfInstallments,
                    BaseAmount = Math.Round(orderAmount / numberOfInstallments, 2),
                    InterestAmount = interestAmount
                };

                if (numberOfInstallments == 1 || opt.Amount >= config.Payment.PaymentGateway.Installments.MinInstallmentAmount)
                    options.Add(opt);
            }

            return options;
        }

        private decimal CalculateInstallmentFeesIfHasConfig(int numberOfInstallments)
        {
            if (config.Payment.Moip.Options.FeesConfig.InstallmentFees.Any(x => x > 0))
                return config.Payment.Moip.Options.FeesConfig.InstallmentFees[numberOfInstallments - 1] / 100m;
            else
                return config.Payment.Moip.Options.FeesConfig.BaseFee / 100m;
        }

        private void SetValidPaymentMethod(Checkout checkout)
        {
            var paymentConfig = config.Payment;

            var boletoEnabled = paymentConfig.Enabled && paymentConfig.PaymentGateway.Boleto.Enabled;
            var creditCardEnabled = paymentConfig.Enabled && paymentConfig.PaymentGateway.CreditCard.Enabled;
            var offlinePaymentEnabled = config.Product.Order.OfflinePaymentEnabled;

            if (!boletoEnabled && !creditCardEnabled && !offlinePaymentEnabled)
                throw new Exception("Nenhum método de pagamento foi habilitado. Contate o administrador da plataforma.");

            var availableMethods = new List<PaymentMethod>();
            if (boletoEnabled) availableMethods.Add(PaymentMethod.Boleto);
            if (creditCardEnabled) availableMethods.Add(PaymentMethod.CreditCard);
            if (offlinePaymentEnabled) availableMethods.Add(PaymentMethod.Offline);

            if (!availableMethods.Any(x => x == checkout.Payment.Method))
                checkout.Payment.Method = availableMethods.First();
        }

        private void SetValidShippingMethod(Cart cart)
        {
            var actualNoShipping = true;
            var noShipping = true;
            var pac = true;
            var sedex = true;
            var asapLog = true;
            var totalExpress = true;
            var privateFleet = true;
            var onSitePickup = true;

            if (config.Product.ShippingMode == ShippingMode.PerProduct)
            {
                foreach (var item in cart.Items.Select(x => x.Product))
                {
                    if (!item.ActualNoShippingEnabled || !config.Product.ActualNoShipping.Enabled)
                        actualNoShipping = false;

                    if (!item.NoShippingEnabled || !config.Product.NoShipping.Enabled)
                        noShipping = false;

                    if (!item.PACEnabled || !config.Product.PACShipping.Enabled)
                        pac = false;

                    if (!item.SedexEnabled || !config.Product.SedexShipping.Enabled)
                        sedex = false;

                    if (!item.PrivateFleetEnabled || !config.Product.PrivateFleetShipping.Enabled)
                        privateFleet = false;

                    if (!item.OnSitePickupEnabled || !config.Product.OnSitePickupShipping.Enabled)
                        onSitePickup = false;
                }
            }
            else
            {
                // Not supported in this mode:
                actualNoShipping = false;
                noShipping = false;

                foreach (var item in cart.Sellers.Select(x => x.Seller.Store))
                {
                    if (!item.PacEnabled || !config.Product.PACShipping.Enabled)
                        pac = false;

                    if (!item.SedexEnabled || !config.Product.SedexShipping.Enabled)
                        sedex = false;

                    if (!item.AsapLogEnabled || !config.Product.AsapLogShipping.Enabled)
                        asapLog = false;

                    if (!item.TotalExpressEnabled || !config.Product.TotalExpressShipping.Enabled)
                        totalExpress = false;

                    if (!item.PrivateFleetEnabled || !config.Product.PrivateFleetShipping.Enabled)
                        privateFleet = false;

                    if (!item.OnSitePickupEnabled || !config.Product.OnSitePickupShipping.Enabled)
                        onSitePickup = false;
                }
            }

            if ((cart.ShippingMethod == ShippingMethod.ActualNoShipping && actualNoShipping) ||
                (cart.ShippingMethod == ShippingMethod.NoShipping && noShipping) ||
                (cart.ShippingMethod == ShippingMethod.PostalRegular && pac) ||
                (cart.ShippingMethod == ShippingMethod.PostalExpress && sedex) ||
                (cart.ShippingMethod == ShippingMethod.AsapLog && asapLog) ||
                (cart.ShippingMethod == ShippingMethod.TotalExpress && totalExpress) ||
                (cart.ShippingMethod == ShippingMethod.PrivateFleet && privateFleet) ||
                (cart.ShippingMethod == ShippingMethod.OnSitePickup && onSitePickup))
                return;

            if (actualNoShipping)
                cart.ShippingMethod = ShippingMethod.ActualNoShipping;
            else if (noShipping)
                cart.ShippingMethod = ShippingMethod.NoShipping;
            else if (pac)
                cart.ShippingMethod = ShippingMethod.PostalRegular;
            else if (sedex)
                cart.ShippingMethod = ShippingMethod.PostalExpress;
            else if (asapLog)
                cart.ShippingMethod = ShippingMethod.AsapLog;
            else if (totalExpress)
                cart.ShippingMethod = ShippingMethod.TotalExpress;
            else if (privateFleet)
                cart.ShippingMethod = ShippingMethod.PrivateFleet;
            else if (onSitePickup)
                cart.ShippingMethod = ShippingMethod.OnSitePickup;
        }

        private OperationResult UpdateCartShipping(Cart cart)
        {
            ConsolidateCartSellers(cart);
            cart.Shipping = 0M;

            var errors = new ErrorDictionary();
            var adapter = shippingService.GetAdapter(cart.ShippingMethod);

            switch (cart.ShippingMode)
            {
                case ShippingMode.PerProduct:
                    {
                        foreach (var seller in cart.Sellers) seller.SellerShipping = 0M;
                        cart.Shipping = cart.Items.Sum(x => x.Shipping);
                        if ((cart.ShippingMethod == ShippingMethod.PostalRegular || cart.ShippingMethod == ShippingMethod.PostalExpress || cart.ShippingMethod == ShippingMethod.AsapLog || cart.ShippingMethod == ShippingMethod.TotalExpress) && !cart.IsEmpty)
                            cart.DeliveryDate = adapter.CalculateDeliveryDate(cart.Items.Max(x => x.DeliveryDays));
                        else
                            cart.DeliveryDate = null;
                        break;
                    }
                case ShippingMode.PerStore:
                    {
                        foreach (var cartSeller in cart.Sellers)
                        {
                            var products = cart.Items
                                .Where(p => p.Product.Store_SellerId == cartSeller.Seller.Id)
                                .Select(i => new ProductWithQty(i.Product, i.Quantity, i.RentalInfo.StartDate))
                                .ToList();

                            var result = adapter.GetShippingForSeller(
                                products,
                                cartSeller.Seller.Store,
                                cart.ShippingAddress);

                            if (!result.OK)
                            {
                                errors.Merge(result.Errors);
                                continue;
                            }

                            cartSeller.SellerShipping = result.Result.TotalShippingPrice;
                            cart.Shipping += cartSeller.SellerShipping;
                            if (cart.ShippingMethod == ShippingMethod.PostalRegular || cart.ShippingMethod == ShippingMethod.PostalExpress || cart.ShippingMethod == ShippingMethod.AsapLog || cart.ShippingMethod == ShippingMethod.TotalExpress)
                                cartSeller.SellerDeliveryDays = result.Result.TotalDeliveryDays;
                            else
                                cart.DeliveryDate = null;
                        }

                        if (cart.Sellers.Any() &&
                            (cart.ShippingMethod == ShippingMethod.PostalRegular || cart.ShippingMethod == ShippingMethod.PostalExpress || cart.ShippingMethod == ShippingMethod.AsapLog || cart.ShippingMethod == ShippingMethod.TotalExpress))
                            cart.DeliveryDate = adapter.CalculateDeliveryDate(cart.Sellers.Max(x => x.SellerDeliveryDays));
                        else
                            cart.DeliveryDate = null;
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            cartRepository.Update(cart);
            return errors;
        }
    }
}