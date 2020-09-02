using Application.Addresses;
using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.QuoteBillingModule.Interfaces;
using Domain.Services;
using Domain.Services.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.PaymentMethods;
using Presentation.Globalization;
using Presentation.Helpers;
using Presentation.Models;
using Presentation.Models.ProductViewModels;
using Presentation.Views.Products.Cart;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.StoreModule.Queries;

namespace Presentation.Controllers.Products
{
    [AuthorizeBuyer]
    public class CartController : BaseController
    {
        private const string CheckoutConfirmationDataKey = "CheckoutConfirmationData";

        private const string AntifraudSessionIdKey = "__CartCheckout__AntifraudSessionId";
        private const string ViewPathFmt = "~/Views/Products/Cart/{0}.cshtml";
        private readonly IAddressAppService addressAppService;
        private readonly ICartQueries cartQueries;
        private readonly IQuoteBillingRepository quoteBillingRepository;
        private readonly BuyerService buyerService;
        private readonly CartService cartService;
        private readonly OrderService orderService;
        private readonly ITimeZoneProvider timeZoneProvider;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly ICustomFieldQueries customFieldQueries;
        private readonly ICustomFieldRepository customFieldRepository;

        public CartController(
            IAddressAppService addressAppService,
            ICartQueries cartQueries,
            IQuoteBillingRepository quoteBillingRepository,
            BuyerService buyerService,
            CartService cartService,
            OrderService orderService,
            ITimeZoneProvider timeZoneProvider,
            ICustomFieldAppService customFieldAppService,
            ICustomFieldQueries customFieldQueries,
            ICustomFieldRepository customFieldRepository)
        {
            this.addressAppService = addressAppService;
            this.cartQueries = cartQueries;
            this.quoteBillingRepository = quoteBillingRepository;
            this.buyerService = buyerService;
            this.cartService = cartService;
            this.orderService = orderService;
            this.timeZoneProvider = timeZoneProvider;
            this.customFieldAppService = customFieldAppService;
            this.customFieldQueries = customFieldQueries;
            this.customFieldRepository = customFieldRepository;
        }

        public ActionResult Index()
        {
            ViewBag.OpenNewShippingAddressModal = false;
            return View(string.Format(ViewPathFmt, "Index"));
        }

        public ActionResult IndexCartWithViewModel()
        {
            var model = GetCartIndexViewModel();
            return View(string.Format(ViewPathFmt, "IndexCartWithViewModel"), model);
        }

        [HttpPost]
        public ActionResult IndexCartWithViewModel(CartWithExtraInfo extraInfoModel)
        {
            if (ModelState.IsValid)
            {
                if (MaaSConfig.Current.Product.Order.ExtraInfoField.Enabled)
                    cartService.SetExtraInfo(GetPersonId(), extraInfoModel.ExtraInfo);

                return RedirectToAction(nameof(Checkout));
            }

            var model = GetCartIndexViewModel(extraInfoModel);
            return View(string.Format(ViewPathFmt, "IndexCartWithViewModel"), model);
        }

        private CartViewModel GetCartIndexViewModel(CartWithExtraInfo extraInfo = null)
        {
            var buyerId = GetPersonId();
            var cart = cartService.Get(buyerId);
            var buyer = buyerService.GetWithAddresses(buyerId);

            ViewBag.OpenNewShippingAddressModal = false;

            var validationResult = cart.IsValidForCheckout();
            if (validationResult.IsFailure)
                ModelState.AddErrors(validationResult);

            if (cart.StockAndShippingValidation.Result != null)
                CheckAndSetShippingModelError(validationResult, cart.StockAndShippingValidation.Result);

            var cartViewModel = new CartViewModel(buyer.GetAllAddresses(), cart, ViewData, extraInfo);

            if (MaaSConfig.Current.CustomFields.CartItem.Enabled)
            {
                cartViewModel.CustomFieldDefinitions = customFieldRepository.DictionaryByEntityType(cartViewModel.EntityType);
                foreach (var item in cartViewModel.CartItems)
                {
                    cartViewModel.CustomFields.AddRange(customFieldQueries.ReadValuesForEntityId(Domain.Entities.CustomFieldEntityType.CartItem, item.Id));
                }
                if (cartViewModel.CustomFields != null || cartViewModel.CustomFields.Count != 0)
                    cartViewModel.CustomFieldsNames = cartViewModel.CustomFields.DistinctBy(x => x.Name).Select(x => x.Name).ToList();
            }

            return cartViewModel;
        }

        [HttpPost]
        public ActionResult AddItem(Guid id, int quantity, DateTime? rentalStartDateTime, DateTime? rentalEndDateTime, int multiplier = 0, int? stockVariant = null, Guid? productAvailabilityId = null, string customFieldsIds = null, string customFieldsValues = null)
        {
            var rentalStartDateTimeUtc = timeZoneProvider.ToUtc(rentalStartDateTime);
            var rentalEndDateTimeUtc = timeZoneProvider.ToUtc(rentalEndDateTime);

            var customFieldsIdsList = JsonConvert.DeserializeObject<List<string>>(customFieldsIds);
            var customFieldsValuesList = JsonConvert.DeserializeObject<List<string>>(customFieldsValues);

            CartCustomFieldsViewModel cartCustomFields = new CartCustomFieldsViewModel();
            if (MaaSConfig.Current.CustomFields.CartItem.Enabled && customFieldsIds != null)
            {
                for (int i = 0; i < customFieldsIdsList.Count; i++)
                {
                    cartCustomFields.CustomFields.Add(customFieldsIdsList[i], customFieldsValuesList[i]);
                }

                customFieldAppService.LoadFieldDefinitionsFor(cartCustomFields);
                var validationCustomFields = customFieldAppService.ValidateModel(cartCustomFields);
                var validationCustom = new OperationResult<Cart>();
                if (validationCustomFields.IsFailure)
                {
                    validationCustom.AddErrors(validationCustomFields.Errors.First().Key, validationCustomFields.Errors.First().Value);
                    return Content(validationCustom.ToJson(), "application/json");
                }
            }
            var validation = cartService.AddItemWithoutShipping(GetPersonId(), id, quantity, rentalStartDateTimeUtc, rentalEndDateTimeUtc, multiplier, stockVariant, productAvailabilityId, cartCustomFields);

            return Content(validation.ToJson(), "application/json");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddItemAndProceedToLogin(Guid id, int quantity, DateTime? rentalStartDateTime, DateTime? rentalEndDateTime, int multiplier = 0, int? stockVariant = null, Guid? productAvailabilityId = null, string cartCustomFieldIds = null, string cartCustomFieldsValues = null)
        {
            var customFieldsIdsList = JsonConvert.DeserializeObject<List<string>>(cartCustomFieldIds);
            var customFieldsValuesList = JsonConvert.DeserializeObject<List<string>>(cartCustomFieldsValues);

            CartCustomFieldsViewModel cartCustomFields = new CartCustomFieldsViewModel();
            var validation = new OperationResult<Cart>();
            if (MaaSConfig.Current.CustomFields.CartItem.Enabled && cartCustomFieldIds != null)
            {
                for (int i = 0; i < customFieldsIdsList.Count; i++)
                {
                    cartCustomFields.CustomFields.Add(customFieldsIdsList[i], customFieldsValuesList[i]);
                }

                customFieldAppService.LoadFieldDefinitionsFor(cartCustomFields);
                var validationCustomFields = customFieldAppService.ValidateModel(cartCustomFields);
                if (validationCustomFields.IsFailure)
                {
                    validation.AddErrors(validationCustomFields.Errors.First().Key, validationCustomFields.Errors.First().Value);
                    return Content(validation.ToJson(), "application/json");
                }
            }

            CartHelper.StoreItemForLogin(id, quantity, rentalStartDateTime, rentalEndDateTime, multiplier, stockVariant, productAvailabilityId, cartCustomFields.CustomFields);
            return Content(validation.ToJson(), "application/json");
        }

        [HttpPost]
        public ActionResult ChangeQuantity(Guid id, int quantity, int? stockVariant = null, Guid? productAvailabilityId = null)
        {
            var validation = cartService.ChangeQuantityWithoutShipping(GetPersonId(), id, quantity, stockVariant, productAvailabilityId);

            if (!validation.OK)
                this.SetErrorMessage(validation.Errors.ToSimpleString(), true);

            return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        public ActionResult RemoveItem(Guid id, int? stockVariant = null, Guid? productAvailabilityId = null)
        {
            var validation = cartService.RemoveItemWithoutShipping(GetPersonId(), id, stockVariant, productAvailabilityId);

            if (!validation.OK)
            {
                this.SetErrorMessage(validation.Errors.ToSimpleString(), true);
            }

            return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
        }

        public ActionResult Checkout()
        {
            var buyerId = GetPersonId();
            var buyer = buyerService.GetWithAddresses(buyerId);

            var paymentRegistrationResult = buyerService.RegisterInPaymentServiceIfNotRegistered(buyer, buyer.Address);
            if (!paymentRegistrationResult.OK) return RedirectToAction("Index", "Manage", new { lang = Request.GetCurrentLang(), message = "CompleteData" });

            var checkoutModel = cartService.GetCheckout(buyerId);
            if (checkoutModel.Cart.IsEmpty) return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });

            if (checkoutModel.CartValidation.HasErrors) ModelState.AddErrors(checkoutModel.CartValidation);

            if (Session[AntifraudSessionIdKey] == null) Session[AntifraudSessionIdKey] = Guid.NewGuid().ToString();

            string shippingWarningMessage = GetShippingWarningMessage(checkoutModel.Cart.StockAndShippingValidation.Result);
            CheckAndSetShippingModelError(new OperationResult(), checkoutModel.Cart.StockAndShippingValidation.Result);

            if (checkoutModel.Cart.StockAndShippingValidation.Result.Any(x => x.ResultType == CorreiosResultType.CommunicationError && !x.IsDefaultShipping))
                ModelState.AddModelError("", checkoutModel.Cart.StockAndShippingValidation.Result.First().Message);

            var viewModel = CheckoutVM.From(shippingWarningMessage, checkoutModel, (string)Session[AntifraudSessionIdKey]);

            return View(string.Format(ViewPathFmt, "Checkout"), viewModel);
        }

        private string GetShippingWarningMessage(IEnumerable<CorreiosShippingDto> shippingsDto)
        {
            string message = "";
            if (shippingsDto != null)
                foreach (var item in shippingsDto)
                {
                    if (item.ResultType == Domain.StoreModule.Entities.CorreiosResultType.Warning)
                    {
                        message = $"{item.CodeResult} - {item.Message}";
                    }
                }
            return message;
        }

        private void CheckAndSetShippingModelError(OperationResult validation, IEnumerable<CorreiosShippingDto> shippingsDto)
        {
            if (shippingsDto != null)
                foreach (var item in shippingsDto)
                {
                    if (item.ResultType == Domain.StoreModule.Entities.CorreiosResultType.Error)
                    {
                        validation.AddError($"{item.ItemId}", CultureInfo.CurrentCulture.GetStringLocalized("CART_PRODUCT_SHIPPING_ERROR"));
                        ModelState.AddErrors(validation);
                    }
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM viewModel)
        {
            var buyerId = GetPersonId();
            var checkout = cartService.GetCheckout(buyerId);
            var antifraudSessionId = (string)Session[AntifraudSessionIdKey];

            checkout.Payment = viewModel.Payment;

            if (checkout.CartValidation.HasErrors)
                ModelState.AddErrors(checkout.CartValidation);

            if (ModelState.IsValid && !checkout.Cart.IsEmpty && !checkout.HasShippingError)
            {
                if (checkout.Payment.Method == PaymentMethod.Boleto)
                    checkout.Payment.PostbackUrl = PaymentHelper.GetBoletoPostbackUrl();

                var createOrderResult = orderService.Create(buyerId, antifraudSessionId, checkout.Payment);

                if (createOrderResult.OK)
                {
                    Session.Remove(AntifraudSessionIdKey);
                    Session.Add(CheckoutConfirmationDataKey, createOrderResult.Result);
                    return RedirectToAction("CheckoutComplete");
                }

                this.AddErrors(createOrderResult, true);
            }

            string shippingWarningMessage = GetShippingWarningMessage(checkout.Cart.StockAndShippingValidation.Result);
            CheckAndSetShippingModelError(new OperationResult(), checkout.Cart.StockAndShippingValidation.Result);
            viewModel = CheckoutVM.From(shippingWarningMessage, checkout, antifraudSessionId);
            return View(string.Format(ViewPathFmt, "Checkout"), viewModel);
        }

        [HttpGet]
        public ActionResult CheckoutComplete()
        {
            if (!(Session[CheckoutConfirmationDataKey] is List<Order> data))
                return RedirectToAction("Index", "MyOrders");

            Session.Remove(CheckoutConfirmationDataKey);
            var confirmationViewModel = new CheckoutConfirmationViewModel(data);
            return View(string.Format(ViewPathFmt, "CheckoutConfirmation"), confirmationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewShippingAddress(AddressRegisterModel addressModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                addressModel.PersonId = GetPersonId();
                addressAppService.AddNewShippingAddress(addressModel);

                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
            }

            var model = GetCartIndexViewModel();
            model.NewShippingAddress = addressModel;

            return View(string.Format(ViewPathFmt, "Index"), model);
        }

        [HttpPost]
        public ActionResult DeleteShippingAddress(string shippingAddressId)
        {
            var buyerId = GetPersonId();
            var cart = cartService.Get(buyerId);
            var buyer = buyerService.GetWithAddresses(buyerId);

            var listAddress = AddShippingAddressList(buyer.GetAllAddresses(), cart);
            if (listAddress.Count <= 1)
                return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
            if (cart.ShippingAddress.Id.ToString() == shippingAddressId)
                cart.ShippingAddress = listAddress.Find(x => x.Id.ToString() != shippingAddressId);

            addressAppService.DeleteShippingAddressAndAddress(Guid.Parse(shippingAddressId));
            return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
        }

        private List<Address> AddShippingAddressList(List<Address> shippingAddresses, Cart cart)
        {
            if (!shippingAddresses.Contains(cart.ShippingAddress))
                shippingAddresses.Add(cart.ShippingAddress);

            return shippingAddresses;
        }

        [HttpPost]
        public ActionResult ChooseShippingAddress(Guid id)
        {
            var buyerId = GetPersonId();
            cartService.SetShippingAddress(buyerId, id);
            return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        public ActionResult ChooseShippingMethod(ShippingMethod method)
        {
            var buyerId = GetPersonId();
            cartService.SetShippingMethod(buyerId, method);
            return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        public ActionResult ChooseCouponCode(string couponCode)
        {
            var buyerId = GetPersonId();
            cartService.SetCoupon(buyerId, couponCode);
            return RedirectToAction("Index", "Cart", new { lang = Request.GetCurrentLang() });
        }

        public JsonResult CountProducts()
        {
            if (Request.IsAuthenticated)
            {
                if (GetLoginProfile().IsBuyer())
                {
                    var count = cartQueries.GetProductsCount(GetPersonId());

                    return Json(new
                    {
                        hasProducts = count > 0,
                        productsCount = count
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                hasProducts = false,
                productsCount = 0
            }, JsonRequestBehavior.AllowGet);
        }
    }
}