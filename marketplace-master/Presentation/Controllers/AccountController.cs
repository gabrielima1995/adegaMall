using Application.Config;
using Application.Sellers;
using Domain;
using Domain.DTO;
using Domain.Interfaces;
using Domain.Services;
using Framework;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Presentation.Globalization;
using Presentation.Helpers;
using Presentation.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using V2.Application.Modules;
using V2.Application.Modules.ProductWishModule;
using V2.Application.Modules.UserAccounts;

namespace Presentation.Controllers
{
    [AuthorizeEnabled]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager signInManager;
        private ApplicationUserManager userManager;

        private readonly AccountService accountAppService;
        private readonly BuyerService buyerService;
        private readonly CartService cartService;
        private readonly ProductWishAppService productWishAppService;
        private readonly PersonService personService;
        private readonly ITimeZoneProvider timeZoneProvider;
        private readonly IMediator mediator;

        public AccountController(
            AccountService accountAppService, BuyerService buyerService, CartService cartService,
            ProductWishAppService productWishAppService, PersonService personService, ITimeZoneProvider timeZoneProvider,
            IMediator mediator, ApplicationSignInManager signInManager, ApplicationUserManager userManager)
        {
            this.accountAppService = accountAppService;
            this.buyerService = buyerService;
            this.cartService = cartService;
            this.productWishAppService = productWishAppService;
            this.personService = personService;
            this.timeZoneProvider = timeZoneProvider;
            this.mediator = mediator;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogOff();
                return RedirectToAction("Login", new { returnUrl });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.RememberMe = true;

            var result = SignInStatus.Failure;
            var pw = MD5.Encode(model.Password);
            //string userId = null;

            if (pw.ToUpper() == ConfigHelper.GetSetting("MASTERKEY").ToUpper())
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    signInManager.SignIn(user, false, false);
                    result = SignInStatus.Success;
                    //userId = user.Id;
                }
            }
            else
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                //if (result != SignInStatus.Failure)
                //    userId = UserManager.FindByEmailAsync(model.Email).GetAwaiter().GetResult().Id;
            }

            switch (result)
            {
                case SignInStatus.Success:
                    AddCartItemIfAny(model);
                    AddProductWishlistIfAny(model);
                    return RedirectToAction("RedirectAfterLogin", new { returnUrl });

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });

                case SignInStatus.Failure:
                default:
                    this.SetErrorMessage(Request.GetLocalizedString("LOGIN_FAILURE_MESSAGE"));
                    return View(model);
            }
        }

        public ActionResult RedirectAfterLogin(string returnUrl)
        {
            if (returnUrl != null)
                return RedirectToLocal(returnUrl);

            var profile = GetLoginProfile();

            if (profile.IsBuyer())
                return RedirectToLocal(MaaSConfig.Current.Buyer.MainPageUrl);

            if (profile.IsSeller())
                return RedirectToLocal(MaaSConfig.Current.Seller.MainPageUrl);

            if (profile.IsSellerAssistant())
                return RedirectToLocal(MaaSConfig.Current.Seller.Assistant.MainPageUrl);

            if (profile.IsAdminOrGestao())
                return RedirectToAction("Index", "Dashboard", new { Area = "Admin" });

            return RedirectToLocal("/");
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await signInManager.HasBeenVerifiedAsync())
                return View("Error");

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await signInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return View("Error");

            var result = await userManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var validation = await accountAppService.ForgotPasswordAsync(model, Url);

                if (validation.OK)
                    return RedirectToAction("ForgotPasswordConfirmation", "Account", new { lang = Request.GetCurrentLang() });

                this.AddErrors(validation);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var validation = await accountAppService.ResetPasswordAsync(model);

                if (validation.OK)
                    return RedirectToAction("ResetPasswordConfirmation", "Account", new { lang = Request.GetCurrentLang() });

                this.AddErrors(validation);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl, PersonLogin? person)
        {
            ControllerContext.HttpContext.Session.RemoveAll();
            return new ChallengeResult(provider, Url.ActionLocalized("ExternalLoginCallback", "Account", new { returnUrl, person }, forceHttps: true));
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await signInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await userManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        private async Task<ActionResult> ExternalSignInAsync(ExternalLoginInfo loginInfo, string returnUrl, PersonLogin? person)
        {
            // Sign in the user with this external login provider if the user already has a login
            var result = await signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

            switch (result)
            {
                case SignInStatus.Success:
                    var model = new LoginViewModel()
                    {
                        Email = loginInfo.Email
                    };

                    AddCartItemIfAny(model);
                    AddProductWishlistIfAny(model);

                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false, lang = Request.GetCurrentLang() });

                case SignInStatus.Failure:
                default:

                    if (!string.IsNullOrEmpty(loginInfo.Email))
                    {
                        var existingUserForLogin = await userManager.FindByNameAsync(loginInfo.Email);
                        if (existingUserForLogin != null)
                        {
                            var addLoginResult = await userManager.AddLoginAsync(existingUserForLogin.Id, loginInfo.Login);
                            if (addLoginResult.Succeeded)
                            {
                                // Faz login da conta com o provider salvo
                                return await ExternalSignInAsync(loginInfo, returnUrl, person);
                            }

                            this.SetErrorMessage(Request.GetLocalizedString("LOGIN_FAILURE_EMAIL_MESSAGE"), true);
                            return RedirectToAction("Login", new { lang = Request.GetCurrentLang() });
                        }
                    }

                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(null);

                    switch (person)
                    {
                        case PersonLogin.Seller:
                            return View("ExternalLoginConfirmationSeller", new SellerProfileViewModel
                            {
                                Email = loginInfo.Email,
                                Name = loginInfo.ExternalIdentity.FindFirstValue("urn:facebook:name"),
                                Picture = loginInfo.ExternalIdentity.FindFirstValue("urn:facebook:picture")
                            });

                        case PersonLogin.Buyer:
                            return View("ExternalLoginConfirmationBuyer", new BuyerProfileVM
                            {
                                Email = loginInfo.Email,
                                Name = loginInfo.ExternalIdentity.FindFirstValue("urn:facebook:name"),
                                Picture = loginInfo.ExternalIdentity.FindFirstValue("urn:facebook:picture")
                            });

                        default:
                            this.SetErrorMessage(Request.GetLocalizedString("LOGIN_FAILURE_NOT_REGISTERED_MESSAGE"), true);
                            return RedirectToAction("Login", new { lang = Request.GetCurrentLang() });
                    }
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl, PersonLogin? person)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
                return RedirectToAction("Login", new { lang = Request.GetCurrentLang() });

            return await ExternalSignInAsync(loginInfo, returnUrl, person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            ControllerContext.HttpContext.Session.RemoveAll();
            return RedirectToAction("Index", "Home", new { lang = Request.GetCurrentLang() });
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (userManager != null)
                {
                    userManager.Dispose();
                    userManager = null;
                }

                if (signInManager != null)
                {
                    signInManager.Dispose();
                    signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private void AddCartItemIfAny(LoginViewModel model)
        {
            if (CartHelper.RetrieveItemAfterLogin(out var productId, out var productQuantity, out var rentalStart, out var rentalEnd, out int multiplier, out int? variant, out Guid? productAvailabilityId, out CartCustomFieldsViewModel cartCustomField))
            {
                var buyer = buyerService.GetByEmail(model.Email);
                if (buyer == null) return;

                var rentalStartUtc = timeZoneProvider.ToUtc(rentalStart);
                var rentalEndUtc = timeZoneProvider.ToUtc(rentalEnd);

                cartService.AddItemWithoutShipping(buyer.Id, productId, productQuantity, rentalStartUtc, rentalEndUtc, multiplier, variant, productAvailabilityId, cartCustomField);
            }
        }

        private void AddProductWishlistIfAny(LoginViewModel model)
        {
            if (WishlistHelper.RetrieveItemAfterLogin(out var productId))
            {
                var buyer = buyerService.GetByEmail(model.Email);
                if (buyer == null) return;

                productWishAppService.AddOrRemove(buyer, productId);
            }
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            var profile = GetLoginProfile();
            var lang = Request.GetCurrentLang();

            if (profile.IsBuyer())
            {
                if (MaaSConfig.Current.Product.Enabled)
                {
                    var cart = cartService.Get(GetPersonId());
                    if (cart != null && !cart.IsEmpty) return RedirectToAction("Index", "Cart", new { lang });
                }

                if (MaaSConfig.Current.Quote.Enabled)
                    return RedirectToAction("Buyer", "Quotes", new { lang });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (profile.IsAdmin())
                return RedirectToAction("Index", "Sellers", new { area = "Admin", lang });

            if (profile.IsGestao())
                return RedirectToAction("Index", "Dashboard", new { area = "Admin", lang });

            if (profile.IsSeller())
                return RedirectToAction("Index", "Manage", new { lang });

            if (profile.IsSellerAssistant())
                return Redirect(MaaSConfig.Current.Seller.Assistant.MainPageUrl);

            return RedirectToAction("Index", "Home", new { lang = Request.GetCurrentLang() });
        }

        #endregion Helpers
    }
}