using Application.Config;
using Application.Sellers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Finance.Interfaces;
using Domain.Services;
using Framework;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Suppliers;
using Presentation.Globalization;
using Presentation.Helpers;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.SellerAssistantModule;
using V2.Application.Modules.UserAccounts;
using V2.Infrastructure.Modules.Metrics;

namespace Presentation.Controllers
{
    [AuthorizeBuyerOrSellerOrSellerAssistant]
    public class ManageController : BaseController
    {
        private const string XsrfKey = "XsrfId";

        private readonly BuyerService buyerService;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly IPaymentOperations paymentOperations;
        private readonly IMetricsQueries metricsQueries;
        private readonly IRepository<Person> personRepository;
        private readonly SellerAppService sellerAppService;
        private readonly ISellerRepository sellerRepository;
        private readonly ISupplierOperations supplierOperations;
        private readonly IGatewayRegistrationLogQueries registrationLogQueries;
        private readonly ApplicationSignInManager signInManager;
        private readonly ApplicationUserManager userManager;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IMediator mediator;
        private readonly IRepository<ApplicationUser> userRepository;
        private readonly IUserAuthProvider authProvider;

        public ManageController(
            BuyerService buyerService, IRepository<Person> personRepository, SellerAppService sellerAppService,
            ISellerRepository sellerRepository, IMetricsQueries metricsQueries, ICustomFieldAppService customFieldAppService,
            IPaymentOperations paymentOperations, ISupplierOperations supplierOperations, IGatewayRegistrationLogQueries registrationLogQueries,
            ApplicationSignInManager signInManager, ApplicationUserManager userManager, IAuthenticationManager authenticationManager,
            IMediator mediator, IRepository<ApplicationUser> userRepository, IUserAuthProvider authProvider)
        {
            this.buyerService = buyerService;
            this.personRepository = personRepository;
            this.sellerAppService = sellerAppService;
            this.sellerRepository = sellerRepository;
            this.metricsQueries = metricsQueries;
            this.customFieldAppService = customFieldAppService;
            this.paymentOperations = paymentOperations;
            this.supplierOperations = supplierOperations;
            this.registrationLogQueries = registrationLogQueries;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.authenticationManager = authenticationManager;
            this.mediator = mediator;
            this.userRepository = userRepository;
            this.authProvider = authProvider;
        }

        [HttpGet]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.BalanceView)]
        public ActionResult Balance()
        {
            var seller = sellerRepository.GetById(GetPersonId());
            var funds = supplierOperations.GetAvailableFunds(seller);
            if (!funds.OK)
                return HttpNotFound();

            return View("Balance", funds.Result);
        }

        [AuthorizeBuyer]
        public ActionResult Buyer()
        {
            var buyerId = GetPersonId();
            var buyer = buyerService.GetProfile(buyerId);
            var model = new BuyerProfileVM(buyer);

            if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
            {
                model.Id = buyerId;
                customFieldAppService.ReadCustomFieldsToViewModel(model);
            }
            if (TempData.ContainsKey("ErrorMessage"))
            {
                foreach (var item in TempData.Where(x => x.Key.Equals("ErrorMessage")))
                {
                    ModelState.AddModelError(item.Key.ToString(), item.Value.ToString());
                }
            }

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(buyer.PreferredTimeZoneId);
            return View("Buyer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeBuyer]
        public ActionResult Buyer(BuyerProfileVM model)
        {
            if (ModelState.IsValid)
            {
                var buyer = buyerService.GetById(GetPersonId());

                model.CopyTo(buyer);

                var updateResult = buyerService.Update(buyer, GetProfileImage(model));

                if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
                {
                    model.Id = buyer.Id;
                    customFieldAppService.LoadFieldDefinitionsFor(model);
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
                    this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                    return RedirectToActionWithLang("Index", "Manage");
                }

                ModelState.AddErrors(updateResult);
            }

            if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
            {
                (model as IWithCustomFields).Id = new Guid(GetPersonId());
                customFieldAppService.ReadCustomFieldsToViewModel(model);
            }

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(model.PreferredTimeZoneId);
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            var appUser = userManager.FindById(User.Identity.GetUserId());
            if (appUser.LoginMode != LoginMode.Password)
                return RedirectToActionWithLang("Index", "Manage");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var appUser = userManager.FindById(User.Identity.GetUserId());
            if (appUser.LoginMode != LoginMode.Password)
                return RedirectToActionWithLang("Index", "Manage");

            var result = await userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                this.SetSuccessMessage(Request.GetLocalizedString("CHANGE_PASSWORD_SUCCESS"));
                return View();
            }

            AddErrors(result);
            return View(model);
        }

        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.DashboardView)]
        public ActionResult Dashboard(DateTime? startDate, DateTime? endDate)
        {
            if (!MaaSConfig.Current.Dashboard.SellerDashboard.Enabled)
                return Seller();

            var sellerId = GetPersonId();
            var viewModel = metricsQueries.RunSellerQuery(startDate, endDate, new Guid(sellerId));

            return View("Dashboard", viewModel);
        }

        public ActionResult Index(ManageMessageId? message)
        {
            var errorMessage = message == ManageMessageId.Error
                ? "An error has occurred." : message == ManageMessageId.CompleteData
                ? Request.GetLocalizedString("CART_COMPLETE_REGISTER_MESSAGE") : "";

            if (!string.IsNullOrEmpty(errorMessage))
                this.SetErrorMessage(errorMessage, true);

            var profile = GetLoginProfile();

            if (profile.IsBuyer())
                return RedirectToAction("Buyer");

            if (profile.IsSellerAssistant())
                return Redirect(profile.GetAllowedPageForAssistant("/manage/dashboard"));

            return RedirectToAction("Dashboard");
        }

        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.GatewayView)]
        public ActionResult Gateway()
        {
            var sellerId = GetPersonId();
            var model = sellerAppService.GetGateway(sellerId);
            return View("SellerGateway", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.GatewayEdit)]
        public ActionResult Gateway(SellerGatewayViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = sellerAppService.RegisterOrUpdateGateway(model);

                if (result.OK)
                {
                    this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                    return RedirectToAction("Gateway", new { lang = Request.GetCurrentLang() });
                }

                ModelState.AddErrors(result);
            }

            return View("SellerGateway", model);
        }

        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProfileView)]
        public ActionResult Seller()
        {
            var sellerId = GetPersonId();
            var seller = sellerAppService.GetProfile(sellerId);

            if (MaaSConfig.Current.CustomFields.Seller.Enabled)
            {
                seller.Id = sellerId;
                customFieldAppService.ReadCustomFieldsToViewModel(seller);
            }

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(seller.PreferredTimeZoneId);
            return View("Seller", seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProfileEdit)]
        public ActionResult Seller(SellerProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = GetPersonId();
                model.Lang = Request.GetCurrentLang();
                ParseAddressCoordinatesIntoModel(model);
                ParseFacebookId(model);

                if (ModelState.IsValid)
                {
                    if (MaaSConfig.Current.CustomFields.Seller.Enabled)
                    {
                        customFieldAppService.LoadFieldDefinitionsFor(model);
                        var result = customFieldAppService.ValidateModel(model);

                        if (result.IsFailure)
                            ModelState.AddErrors(result);
                    }

                    if (ModelState.IsValid)
                    {
                        var updateSellerResult = sellerAppService.Update(model, GetUserId());

                        if (updateSellerResult.OK)
                        {
                            this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                            return User.Identity.GetLoginProfile().IsSellerAssistant()
                                ? Redirect(MaaSConfig.Current.Seller.Assistant.MainPageUrl)
                                : RedirectToActionWithLang("Index", "Manage") as ActionResult;
                        }

                        else ModelState.AddErrors(updateSellerResult);
                    }
                }
            }

            if (MaaSConfig.Current.CustomFields.Seller.Enabled)
            {
                (model as IWithCustomFields).Id = new Guid(GetPersonId());
                customFieldAppService.ReadCustomFieldsToViewModel(model);
            }

            ViewBag.PreferredTimeZoneId = TimeZoneHelper.GetTimeZoneSelectList(model.PreferredTimeZoneId);
            return View(model);
        }

        public async Task<ActionResult> Profiles()
        {
            var actualUserId = GetActualUserId();
            var profiles = await mediator.Send(new ListProfilesForUser(actualUserId));
            return View(new ManageProfileViewModel(profiles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Profiles(ManageProfileViewModel model)
        {
            var actualUserId = GetActualUserId();

            if (model.LoginWithPersonId != null)
            {
                var person = personRepository.Get(model.LoginWithPersonId);
                if (person.UserId == actualUserId)
                {
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    Session.RemoveAll();

                    var user = userRepository.Get(actualUserId);
                    await signInManager.SignInAsync(user, false, false);
                    PreferredPersonIdHelper.Set(person.Id);

                    var redirectUrl =
                        person.PersonType == PersonType.AdminPerson ? "/admin" :
                        person.PersonType == PersonType.Buyer ? MaaSConfig.Current.Buyer.MainPageUrl :
                        person.PersonType == PersonType.Seller ? MaaSConfig.Current.Seller.MainPageUrl :
                        person.PersonType == PersonType.SellerAssistant ? GetLoginProfile().GetAllowedPageForAssistant(MaaSConfig.Current.Seller.Assistant.MainPageUrl) : "/";
                    return Redirect(redirectUrl);
                }

                this.SetErrorMessage("PROFILE_LOGIN_ERROR");
            }

            var profiles = await mediator.Send(new ListProfilesForUser(actualUserId));
            return View(new ManageProfileViewModel(profiles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.BalanceEdit)]
        public ActionResult WithdrawBalance()
        {
            if (!MaaSConfig.Current.Payment.GatewayIsWirecard())
                throw new NotImplementedException();

            var id = GetPersonId();
            var seller = sellerRepository.Get(x => x.Id == id, x => x.BankAccount);
            var result = supplierOperations.TransferBalance(seller, new Accountable(seller), seller.BankAccount);

            if (result.OK)
                return RedirectToAction("Balance");
            else
            {
                TempData.Add("ErrorLog", result.Errors[0].Value);
                return RedirectToAction("Balance");
            }
        }

        [HttpGet]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.BalanceView)]
        public ActionResult ListWithdraws()
        {
            var allRecords = new List<TransferDto>();
            var sellerId = GetPersonId();
            var gatewayRegistrations = registrationLogQueries.ListGatewayIdAndToken(sellerId);

            foreach (var registration in gatewayRegistrations.Result)
            {
                var result = paymentOperations.ListWithdraws(registration.GatewayRecipientId, registration.GatewayApiToken);
                if (result.OK) allRecords.AddRange(result.Result);
            }

            allRecords = allRecords.OrderByDescending(record => record.DateCreated).ToList();
            return Json(ApiResultEnvelope.Success(allRecords), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userManager?.Dispose();
                signInManager?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }

        private Domain.DTO.FileUpload GetProfileImage(BuyerProfileVM model)
        {
            if (model == null || model.NewPicture == null) return null;
            var postedFile = model.NewPicture;
            return new Domain.DTO.FileUpload(postedFile.InputStream, postedFile.FileName, postedFile.ContentType);
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

        private void ParseFacebookId(SellerProfileViewModel model)
        {
            string facebook = model.FacebookProfile;

            if (!string.IsNullOrWhiteSpace(facebook))
            {
                var fullId = new Regex(@"^((http(s)?)?:\/\/)?(www.)?(facebook|fb).com\/(?'fbid'[A-z0-9_\-\.]+)(\/)?$");
                string facebookId = null;

                if (fullId.IsMatch(facebook))
                {
                    var match = fullId.Match(facebook);
                    facebookId = match.Groups["fbid"].Value;
                }

                if (facebookId == null)
                {
                    var onlyId = new Regex(@"^(?'fbid'[A-z0-9_\-\.]+)$");
                    if (onlyId.IsMatch(facebook))
                    {
                        var match = onlyId.Match(facebook);
                        facebookId = match.Groups["fbid"].Value;
                    }
                }

                if (facebookId == null)
                {
                    var idFormat = new Regex(@"id\=([A-z0-9_\-\.]+)");

                    if (idFormat.IsMatch(facebook) && (facebook.Contains("facebook.com", StringComparison.OrdinalIgnoreCase) || facebook.Contains("fb.com", StringComparison.OrdinalIgnoreCase)))
                    {
                        var match = idFormat.Match(facebook);
                        facebookId = match.Groups[1].Value;
                    }
                }

                if (facebookId != null)
                {
                    var facebookUrl = $@"https://www.facebook.com/{facebookId}/";
                    model.FacebookProfile = facebookUrl;
                }
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            CompleteData
        }
    }
}