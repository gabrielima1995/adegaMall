using Application.Modules.Portfolio.SocialMedias;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Presentation.Globalization;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [AuthorizeSeller]
    public class SocialMediaController : BaseController
    {
        private readonly ISocialMediaAppService _socialMediaAppService;
        private ApplicationSignInManager _signInManager;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? OwinContextExtensions.Get<ApplicationSignInManager>(HttpContext.GetOwinContext());
            }
            private set
            {
                _signInManager = value;
            }
        }

        public SocialMediaController(
            ISocialMediaAppService socialMediaAppService)
        {
            _socialMediaAppService = socialMediaAppService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider)
        {
            if (provider.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
            {
                provider = "GooglePlus";
            }

            ControllerContext.HttpContext.Session.RemoveAll();

            return new ChallengeResult(provider, Url.ActionLocalized("ChooseLink", "SocialMedia"));
        }

        public async Task<ActionResult> ChooseLink(string error)
        {
            try
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

                if (loginInfo != null && string.IsNullOrEmpty(error))
                {
                    // Obtem os canais do provedor
                    var validationResult = await _socialMediaAppService.GetChannelsAsync(loginInfo);

                    if (validationResult.OK)
                    {
                        var response = validationResult.Result;

                        // Exibe tela para escolher o canal
                        if (response.Count > 1)
                        {
                            return View(response);
                        }

                        // Vincula automaticamente o canal
                        if (response.Count == 1)
                        {
                            return ChooseLink(response.FirstOrDefault());
                        }
                    }
                    else
                    {
                        this.SetErrorMessage(validationResult.Errors.ToSimpleString(), true);
                    }
                }
            }
            catch (Exception)
            {
                this.SetErrorMessage(Request.GetLocalizedString("EXTERNAL_LOGIN_FAILURE_ERROR"), true);
            }

            return RedirectToAction("Index", "Portfolio", new { lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseLink(SocialMediaChannelViewModel request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    request.SellerId = GetPersonId();

                    _socialMediaAppService.LinkToSeller(request);

                    this.SetSuccessMessage(Request.GetLocalizedString("CHOOSE_LINK_SUCCESS_MESSAGE"), true);

                    return RedirectToAction("Index", "Portfolio", new { lang = Request.GetCurrentLang() });
                }
            }
            catch (Exception)
            {
                // ignored
            }

            this.SetErrorMessage(Request.GetLocalizedString("EXTERNAL_LOGIN_FAILURE_ERROR"), true);

            return RedirectToAction("Index", "Portfolio", new { lang = Request.GetCurrentLang() });
        }

        public ActionResult RemoveLink(string provider)
        {
            try
            {
                var sellerId = GetPersonId();
                _socialMediaAppService.RemoveLinkOfSeller(sellerId, provider);
                this.SetSuccessMessage(Request.GetLocalizedString("REMOVE_LINK_SUCCESS_MESSAGE"), true);
            }
            catch (Exception)
            {
                this.SetErrorMessage(Request.GetLocalizedString("REMOVE_LINK_ERROR_MESSAGE"), true);
            }

            return RedirectToAction("Index", "Portfolio", new { lang = Request.GetCurrentLang() });
        }
    }
}