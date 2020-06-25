using Application.Config;
using Application.Sellers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;
using Framework;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Presentation.Globalization;
using Presentation.Helpers;
using Presentation.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;
using V2.Application.Modules;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.ProductWishModule;
using V2.Application.Modules.UserAccounts;

namespace Presentation.Controllers
{
    [AuthorizeEnabled]
    public class RegisterController : BaseController
    {
        private readonly BuyerService buyerService;
        private readonly CartService cartService;
        private readonly SellerAppService sellerAppService;
        private readonly ProductWishAppService productWishAppService;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly ITimeZoneProvider timeZoneProvider;
        private readonly ApplicationSignInManager signInManager;
        private readonly IAuthenticationManager authentication;
        private readonly IMediator mediator;
        private readonly IUserAuthProvider authProvider;

        public RegisterController(
            BuyerService buyerService,
            CartService cartService,
            SellerAppService sellerAppService,
            ProductWishAppService productWishAppService,
            ICustomFieldAppService customFieldAppService,
            ITimeZoneProvider timeZoneProvider,
            ApplicationSignInManager signInManager,
            IAuthenticationManager authentication,
            IMediator mediator,
            IUserAuthProvider authProvider)
        {
            this.buyerService = buyerService;
            this.cartService = cartService;
            this.sellerAppService = sellerAppService;
            this.productWishAppService = productWishAppService;
            this.customFieldAppService = customFieldAppService;
            this.timeZoneProvider = timeZoneProvider;
            this.signInManager = signInManager;
            this.authentication = authentication;
            this.mediator = mediator;
            this.authProvider = authProvider;
        }

        public ActionResult Warning() => View("Warning");

        [AllowAnonymous]
        public ActionResult Buyer(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? Url.ActionLocalized("Index", "Home");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Buyer(BuyerRegisterWithPasswordVM model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.Lang = Request.GetCurrentLang();
                var userAndProfile = model.GetUserAndProfile();
                var registration = await mediator.Send(new RegisterBuyerUserAndProfile(userAndProfile, model.Password));

                if (registration.OK)
                {
                    await signInManager.SignInAsync(userAndProfile.User, false, false);
                    return RedirectToActionWithLang("BuyerFirstStepOk", "Register", new { returnUrl });
                }

                ModelState.AddErrors(registration);
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdditionalProfile(AdditionalProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var registration = await mediator.Send(model.AsRequest());

                if (registration.OK)
                {
                    authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    Session.RemoveAll();

                    await signInManager.SignInAsync(registration.Result.User, false, false);
                    PreferredPersonIdHelper.Set(registration.Result.Profile.Id);

                    var nextPage = model.NewProfileType == PersonType.Buyer ? "BuyerFirstStepOk" : "SellerFirstStepOk";
                    return RedirectToActionWithLang(nextPage, "Register");
                }

                this.SetErrorMessage(registration.Errors.ToSimpleString());
                ModelState.AddErrors(registration);
            }

            return RedirectToAction("Profiles", "Manage");
        }

        [AuthorizeBuyer]
        public ActionResult BuyerFirstStepOk(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return RedirectToActionWithLang("CompleteBuyer", "Register", new { returnUrl });
        }

        [AuthorizeBuyer]
        public ActionResult CompleteBuyer(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var buyerId = GetPersonId();
            var buyer = buyerService.GetProfile(buyerId);

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(buyer.PreferredTimeZoneId);

            var model = new BuyerProfileVM(buyer);

            if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(model);

            return View(model);
        }

        [HttpPost]
        [AuthorizeBuyer]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteBuyer(BuyerProfileVM model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.Lang = Request.GetCurrentLang();

                var buyerId = GetPersonId();
                var buyer = buyerService.GetById(buyerId);

                model.CopyTo(buyer);

                var fileUpload = model.NewPicture?.GetFileUpload();
                var updateResult = buyerService.Update(buyer, fileUpload);

                if (updateResult.OK && MaaSConfig.Current.CustomFields.Buyer.Enabled)
                {
                    customFieldAppService.LoadFieldDefinitionsFor(model);
                    model.Id = buyerId;

                    var result = customFieldAppService.ValidateModel(model);

                    if (result.OK)
                    {
                        if (model.HasCustomFields())
                            customFieldAppService.StoreCustomFields(model);
                    }
                    else ModelState.AddErrors(result);
                }

                if (updateResult.OK && ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(returnUrl))
                        returnUrl = Url.ActionLocalized("Index", "Home");

                    if (AddCartItemIfAny(buyer))
                        returnUrl = Url.ActionLocalized("Checkout", "Cart");

                    AddProductWishlistIfAny(buyer);

                    return RedirectToAction("BuyerCompleted", new { returnUrl });
                }

                ModelState.AddErrors(updateResult);
            }

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(model.PreferredTimeZoneId);

            if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
            {
                if (model.Id == null)
                    model.Id = GetPersonId();

                customFieldAppService.ReadCustomFieldsToViewModel(model);
            }

            return View(model);
        }

        [AuthorizeBuyer]
        public ActionResult BuyerCompleted(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return RedirectToLocal(returnUrl);

            return RedirectToActionWithLang("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Seller(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? Url.ActionLocalized("Index", "Manage");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Seller(SellerRegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.Lang = Request.GetCurrentLang();
                var registration = await sellerAppService.RegisterAsync(model);

                if (registration.OK)
                {
                    await signInManager.SignInAsync(registration.Result.User, isPersistent: false, rememberBrowser: false);
                    return RedirectToActionWithLang("SellerFirstStepOk", "Register", new { returnUrl });
                }

                ModelState.AddErrors(registration);
            }

            return View(model);
        }

        [AuthorizeSeller]
        public ActionResult SellerFirstStepOk(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return RedirectToActionWithLang("CompleteSeller", "Register");
        }

        [AuthorizeSeller]
        public ActionResult CompleteSeller(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var sellerId = GetLoginProfile().PersonId;
            var model = sellerAppService.GetProfile(sellerId);

            if (MaaSConfig.Current.CustomFields.Seller.Enabled)
            {
                model.Id = sellerId;
                customFieldAppService.ReadCustomFieldsToViewModel(model);
            }

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(model.PreferredTimeZoneId);

            if (MaaSConfig.Current.Seller.SkipToContent.Enabled)
                ViewData.Add("EnableSkip", true);

            return View(model);
        }

        [HttpPost]
        [AuthorizeSeller]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteSeller(SellerProfileViewModel profileViewModel, string returnUrl)
        {
            if (ModelState.IsValid || MaaSConfig.Current.Seller.SavePartialProfiles.Enabled)
            {
                profileViewModel.Id = GetPersonId();
                profileViewModel.Lang = Request.GetCurrentLang();
                ParseAddressCoordinatesIntoModel(profileViewModel);

                var result = OperationResult.Success;

                if (MaaSConfig.Current.CustomFields.Seller.Enabled)
                {
                    customFieldAppService.LoadFieldDefinitionsFor(profileViewModel);
                    result = customFieldAppService.ValidateModel(profileViewModel);

                    if (result.OK)
                    {
                        if (profileViewModel.HasCustomFields())
                            customFieldAppService.StoreCustomFields(profileViewModel);
                    }
                    else
                    {
                        ModelState.AddErrors(result);
                    }

                }

                if (result.OK)
                {
                    var validation = sellerAppService.Update(profileViewModel, GetUserId());
                    if (validation.OK) return RedirectToActionWithLang("SellerCompleted", "Register", new { returnUrl });
                    ModelState.AddErrors(validation);
                }
            }
            else if (MaaSConfig.Current.CustomFields.Seller.Enabled)
                customFieldAppService.LoadFieldDefinitionsFor(profileViewModel);

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(profileViewModel.PreferredTimeZoneId);
            return View(profileViewModel);
        }

        [AuthorizeSeller]
        public ActionResult SellerCompleted(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return RedirectToLocal(returnUrl);

            return RedirectToActionWithLang("Index", "Manage");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmationSeller(SellerProfileViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Manage", new { lang = Request.GetCurrentLang() });

            if (ModelState.IsValid)
            {
                var info = await authentication.GetExternalLoginInfoAsync();
                if (info == null) return View("ExternalLoginFailure");

                model.Lang = Request.GetCurrentLang();
                var userAndProfile = model.GetUserAndProfile();
                var registration = await mediator.Send(new RegisterSellerUserAndProfile(userAndProfile, info.Login));

                if (registration.OK)
                {
                    await sellerAppService.AugumentProfileAsync(userAndProfile, model.Picture);
                    await signInManager.SignInAsync(userAndProfile.User, false, false);
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddErrors(registration);
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(model.PreferredTimeZoneId);

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmationBuyer(BuyerProfileVM model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Manage", new { lang = Request.GetCurrentLang() });

            if (ModelState.IsValid)
            {
                var info = await authentication.GetExternalLoginInfoAsync();
                if (info == null)
                    return View("ExternalLoginFailure");

                model.Lang = Request.GetCurrentLang();
                var userAndProfile = model.GetUserAndProfile();
                var registration = await mediator.Send(new RegisterBuyerUserAndProfile(userAndProfile, info.Login));

                if (registration.OK)
                {
                    var avatar = model.NewPicture?.GetFileUpload();
                    buyerService.AugumentProfile(userAndProfile, avatar);

                    await signInManager.SignInAsync(userAndProfile.User, false, false);
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddErrors(registration);
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(model.PreferredTimeZoneId);

            return View(model);
        }

        private bool AddCartItemIfAny(Buyer buyer)
        {
            if (CartHelper.RetrieveItemAfterLogin(out var productId, out var productQuantity, out var rentalStart, out var rentalEnd, out int multiplier, out int? variant, out Guid? productAvailabilityId, out CartCustomFieldsViewModel cartCustomFields))
            {
                var rentalStartUtc = timeZoneProvider.ToUtc(rentalStart);
                var rentalEndUtc = timeZoneProvider.ToUtc(rentalEnd);

                var result = cartService.AddItemWithoutShipping(buyer.Id, productId, productQuantity, rentalStartUtc, rentalEndUtc, multiplier, variant, productAvailabilityId, cartCustomFields);
                return result.OK;
            }

            return false;
        }

        private bool AddProductWishlistIfAny(Buyer buyer)
        {
            if (WishlistHelper.RetrieveItemAfterLogin(out var productId))
            {
                var result = productWishAppService.AddOrRemove(buyer, productId);
                return result.OK;
            }

            return false;
        }

        private void ParseAddressCoordinatesIntoModel(SellerProfileViewModel model)
        {
            var latStr = Request.Form["AddressLat"];
            var lngStr = Request.Form["AddressLng"];
            var culture = CultureInfo.InvariantCulture;

            if (!string.IsNullOrEmpty(latStr))
                model.AddressLat = double.Parse(latStr, culture).ToString();

            if (!string.IsNullOrEmpty(lngStr))
                model.AddressLng = double.Parse(lngStr, culture).ToString();
        }
    }
}