using Application.Config;
using DevTrends.MvcDonutCaching;
using Domain;
using Domain.Core;
using Domain.DTO;
using Domain.Events.Config;
using Domain.Modules.Customization;
using Domain.Services;
using Framework;
using Presentation.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class CustomizeController : BaseController
    {
        private const string ErrorsKey = "__CUSTOMIZE_ERRORS";

        private readonly IRepository<ApplicationConfig> _appConfigRepository;
        private readonly UploadService _uploadService;
        private readonly CustomizationService _customizationService;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public CustomizeController(
            IRepository<ApplicationConfig> appConfigRepository,
            UploadService uploadService,
            CustomizationService customizationService,
            IDomainEventDispatcher domainEventDispatcher)
        {
            _appConfigRepository = appConfigRepository;
            _uploadService = uploadService;
            _customizationService = customizationService;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public ActionResult Docs()
        {
            if (!CanCustomize()) return HttpNotFound();

            PopulateModelStateWithErrors();
            return View();
        }

        public ActionResult Identity()
        {
            if (!CanCustomize()) return HttpNotFound();

            PopulateModelStateWithErrors();
            return View();
        }

        public ActionResult Home()
        {
            if (!CanCustomize()) return HttpNotFound();

            PopulateModelStateWithErrors();
            return View();
        }

        public ActionResult SellerRegistration()
        {
            if (!CanCustomize()) return HttpNotFound();

            PopulateModelStateWithErrors();
            return View();
        }

        public ActionResult Html()
        {
            if (!MaaSConfig.Current.AdminCustomization.HtmlCustomizationEnabled)
                return HttpNotFound();

            var custom = _customizationService.GetPlatformCustomization();

            return View(new CustomizeHtmlVM
            {
                HtmlBodyEnd = custom.HtmlBodyEnd,
                HtmlBodyStart = custom.HtmlBodyStart,
                HtmlHead = custom.HtmlHead
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Html(CustomizeHtmlVM viewModel)
        {
            if (!MaaSConfig.Current.AdminCustomization.HtmlCustomizationEnabled)
                return HttpNotFound();

            var custom = _customizationService.GetPlatformCustomization();

            if (ModelState.IsValid)
            {
                custom.SetHtmlBodyEnd(viewModel.HtmlBodyEnd);
                custom.SetHtmlBodyStart(viewModel.HtmlBodyStart);
                custom.SetHtmlHead(viewModel.HtmlHead);

                try
                {
                    _customizationService.UpdatePlatformCustomization(custom);
                    return RedirectToAction("Html");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(CustomizeFileInfoVM viewModel)
        {
            if (!CanCustomize()) return HttpNotFound();

            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file.ContentLength > 0)
                    {
                        var fileUpload = new FileUpload(file.InputStream, file.FileName, file.ContentType);
                        var uploadResult = _uploadService.Upload(viewModel.FileType, fileUpload);

                        if (!uploadResult.OK)
                        {
                            SetErrors(uploadResult.Errors);
                        }
                        else
                        {
                            var updateConfigResult = UpdateConfig(viewModel.Option, uploadResult.Result);
                            if (!updateConfigResult.OK) SetErrors(updateConfigResult.Errors);
                            else ClearDonutCache();
                        }
                    }
                }
            }

            var defaultUrl = Url.ActionLocalized("Identity", "Customize", new { Area = "Admin" });
            return Redirect(viewModel.RedirectUrl ?? defaultUrl);
        }

        private bool CanCustomize()
        {
            return MaaSConfig.Current.AdminCustomization.Enabled || GetLoginProfile().IsGestao();
        }

        private void ClearDonutCache()
        {
            var cacheManager = new OutputCacheManager();
            cacheManager.RemoveItems();
        }

        private void PopulateModelStateWithErrors()
        {
            if (HttpContext.Items.Contains(ErrorsKey))
            {
                var errors = HttpContext.Items[ErrorsKey] as ErrorDictionary;
                if (errors != null) ModelState.AddErrors(errors);
            }
        }

        private void SetErrors(ErrorDictionary errors)
        {
            if (errors != null && errors.Count > 0)
                HttpContext.Items.Add(ErrorsKey, errors);
        }

        private OperationResult UpdateConfig(CustomizeOption option, object value)
        {
            if (value != null)
            {
                var config = MaaSConfig.Current;

                switch (option)
                {
                    case CustomizeOption.FavIcon: config.Layout.Images.Favicon = value.ToString(); break;
                    case CustomizeOption.HomeBackground: config.Layout.Images.Background1 = value.ToString(); break;
                    case CustomizeOption.HomeBenefitsIcon1: config.HowItWorks.Items[0].ImageUrl = value.ToString(); break;
                    case CustomizeOption.HomeBenefitsIcon2: config.HowItWorks.Items[1].ImageUrl = value.ToString(); break;
                    case CustomizeOption.HomeBenefitsIcon3: config.HowItWorks.Items[2].ImageUrl = value.ToString(); break;
                    case CustomizeOption.LogoHome: config.Layout.Images.LogoHome = value.ToString(); break;
                    case CustomizeOption.LogoInternal: config.Layout.Images.Logo = value.ToString(); break;
                    case CustomizeOption.PrivacyPolicy: config.Enterprise.PrivacyPolicyUrl = value.ToString(); break;
                    case CustomizeOption.SellerBenefitsIcon1: config.HelpQuestions.Items[0].ImageUrl = value.ToString(); break;
                    case CustomizeOption.SellerBenefitsIcon2: config.HelpQuestions.Items[1].ImageUrl = value.ToString(); break;
                    case CustomizeOption.SellerBenefitsIcon3: config.HelpQuestions.Items[2].ImageUrl = value.ToString(); break;
                    case CustomizeOption.UseTerms: config.Enterprise.TermsOfUseUrl = value.ToString(); break;
                    case CustomizeOption.Background2: config.Layout.Images.Background2 = value.ToString(); break;

                    default: return new OperationResult("", "Invalid CustomizeOption value: " + option.ToString());
                }

                #region Persists and reloads the config object

                var appConfig = _appConfigRepository.Query().FirstOrDefault();

                if (appConfig == null) appConfig = new ApplicationConfig();

                var oldConfig = appConfig.GetConfig();

                appConfig.MaaSConfig = config.ToJson();

                if (appConfig.Id == 0) _appConfigRepository.Insert(appConfig);
                else _appConfigRepository.Update(appConfig);

                _appConfigRepository.SaveChanges();

                MaaSConfig.Load(appConfig);
                _domainEventDispatcher.Dispatch(new ConfigUpdatedEvent(MaaSConfig.Current, oldConfig));

                #endregion Persists and reloads the config object
            }

            return OperationResult.Success;
        }
    }
}