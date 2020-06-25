using Application.Config;
using DevTrends.MvcDonutCaching;
using Domain;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Domain.UserConfig;
using Framework;
using Infra.Crosscutting.Globalization;
using Presentation.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Core;
using Domain.Events.Config;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class NewCustomizeController : Controller
    {
        private const string errorsKey = "__CUSTOMIZE_ERRORS";

        private readonly IRepository<ApplicationConfig> appConfigRepository;
        private readonly IStringRepository stringRepository;
        private readonly ICustomFieldRepository customFieldRepository;
        private readonly IUserConfigRepository userConfigRepository;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public NewCustomizeController(
            IRepository<ApplicationConfig> appConfigRepository,
            IStringRepository stringRepository,
            ICustomFieldRepository customFieldRepository,
            IUserConfigRepository userConfigRepository,
            IDomainEventDispatcher domainEventDispatcher)
        {
            _domainEventDispatcher = domainEventDispatcher;
            this.appConfigRepository = appConfigRepository;
            this.stringRepository = stringRepository;
            this.customFieldRepository = customFieldRepository;
            this.userConfigRepository = userConfigRepository;
        }

        public ActionResult Index()
        {
            return View("Marketplace");
        }

        public ActionResult Marketplace(string lang = "pt-br")
        {
            CustomizeMarketplaceVM model = new CustomizeMarketplaceVM()
            {
                Lang = lang,
                AboutText = GlobalizationHelper.GetString("ABOUT_TEXT"),
                CustomerServiceEmail = MaaSConfig.Current.Enterprise.CustomerServiceEmail,
                DocumentNumber = MaaSConfig.Current.Enterprise.DocumentNumber,
                MarketplaceName = MaaSConfig.Current.ApplicationName,
            };

            return View("Marketplace", model);
        }

        [HttpPost]
        public ActionResult Marketplace(CustomizeMarketplaceVM model)
        {
            if (ModelState.IsValid)
            {
                MaaSConfig.Current.Enterprise.CustomerServiceEmail = model.CustomerServiceEmail;

                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress = ExtractEmail(model.CustomerServiceEmail) ?? MaaSConfig.Current.Environment.Smtp.DefaultFromAddress;

                MaaSConfig.Current.ApplicationName = model.MarketplaceName;
                MaaSConfig.Current.Enterprise.Name = model.MarketplaceName;
                MaaSConfig.Current.Environment.Smtp.DefaultFromName = model.MarketplaceName;
                MaaSConfig.Current.Enterprise.DocumentNumber = model.DocumentNumber;

                var config = appConfigRepository.Query().FirstOrDefault();

                config.MaaSConfig = MaaSConfig.Current.ToJson();
                appConfigRepository.Update(config);
                appConfigRepository.SaveChanges();

                GlobalizationHelper.UpdateString(stringRepository, "HOME_TITLE", model.Lang, model.MarketplaceName);
                GlobalizationHelper.UpdateString(stringRepository, "ABOUT_TEXT", model.Lang, model.AboutText);
            }

            return RedirectToAction("Marketplace");
        }

        public ActionResult Home(string lang = "pt-br")
        {
            var data = GetHomeVM(lang);

            return View("Home", data);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitHome(CustomizeHomeVM homeSubmitModel)
        {
            if (ModelState.IsValid)
            {
                SaveHomeVM(homeSubmitModel, homeSubmitModel.Lang);
                return RedirectToAction("Home");
            }

            return RedirectToAction("Home");
        }

        public ActionResult CreateNewMiscItem()
        {
            MaaSConfig.Current.MiscArea.Items.Add(new Application.Config.Sections.MiscAreaItemConfig());
            SaveConfig(MaaSConfig.Current);
            return RedirectToAction("Home");
        }

        public ActionResult RemoveMiscItem(int index)
        {
            if (MaaSConfig.Current.MiscArea.Items.Count > index)
            {
                MaaSConfig.Current.MiscArea.Items.RemoveAt(index);
                SaveConfig(MaaSConfig.Current);
            }

            return RedirectToAction("Home");
        }

        public ActionResult Identity()
        {
            var model = new CustomizeIdentityVM()
            {
                CurrentPreset = new CustomizeIdentityColorPresetVM()
                {
                    Accent1 = MaaSConfig.Current.Layout.Colors["$color_accent1"],
                    Accent2 = MaaSConfig.Current.Layout.Colors["$color_accent2"],
                    Contrast = MaaSConfig.Current.Layout.Colors["$color_contrast"],
                    MenuHomeBG = MaaSConfig.Current.Layout.Colors["$menu_home_bg"],
                    MenuHomeColorAccent = MaaSConfig.Current.Layout.Colors["$menu_home_color"],
                },
            };

            model.CurrentPreset.SanitizeColors();

            return View("Identity", model);
        }

        [HttpPost]
        public ActionResult Identity(CustomizeIdentityVM viewModel)
        {
            if (ModelState.IsValid)
            {
                MaaSConfig.Current.Layout.Colors["$color_accent1"] = viewModel.CurrentPreset.Accent1;
                MaaSConfig.Current.Layout.Colors["$color_accent2"] = viewModel.CurrentPreset.Accent2;
                MaaSConfig.Current.Layout.Colors["$color_contrast"] = viewModel.CurrentPreset.Contrast;
                MaaSConfig.Current.Layout.Colors["$menu_home_bg"] = viewModel.CurrentPreset.MenuHomeBG;
                MaaSConfig.Current.Layout.Colors["$menu_home_color"] = viewModel.CurrentPreset.MenuHomeColorAccent;
            }

            SaveConfig(MaaSConfig.Current);

            return View("Identity", viewModel);
        }

        public ActionResult Seller(string lang = "pt-br")
        {
            return View("Seller", GetCustomizeSellerVM(lang));
        }

        [HttpPost]
        public ActionResult Seller(CustomizeSellerRegisterVM sellerRegisterVM)
        {
            if (ModelState.IsValid)
                SaveCustomizeSellerVM(sellerRegisterVM);

            return RedirectToAction("Seller", new { lang = sellerRegisterVM.Lang });
        }

        public ActionResult Buyer(string lang)
        {
            return View("Buyer", GetCustomizeBuyerVM(lang));
        }

        [HttpPost]
        public ActionResult Buyer(CustomizeBuyerRegisterVM viewModel)
        {
            if (ModelState.IsValid)
                SaveBuyerRegisterVM(viewModel);

            return RedirectToAction("Buyer", new { lang = viewModel.Lang });
        }

        public ActionResult Others()
        {
            return View("Others");
        }

        public ActionResult UserConfig()
        {
            var model = new CustomizeUserConfigVM();

            model.UserConfig = userConfigRepository.ReadConfig() ?? new UserConfiguration();
            model.CustomLinks.Clear();

            for (int i = 0; i < 3; i++)
            {
                model.CustomLinks.Add(new CustomLinksConfigVM(MaaSConfig.Current.AdditionalLinks[i]));
            }

            return View("Config", model);
        }

        [HttpPost]
        public ActionResult CustomLinks(CustomizeUserConfigVM model)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (model.CustomLinks[i].Enabled && model.CustomLinks[i].Url != null && !ValidateUrl(model.CustomLinks[i].Url))
                    {
                        ModelState.AddModelError("Error", "Um link habilitado possui uma URL Inválida");
                        return View("Config", model);
                    }

                    MaaSConfig.Current.AdditionalLinks[i].Enabled = model.CustomLinks[i].Enabled;
                    MaaSConfig.Current.AdditionalLinks[i].Label = model.CustomLinks[i].Label;
                    MaaSConfig.Current.AdditionalLinks[i].Url = model.CustomLinks[i].Url;
                }

                SaveConfig(MaaSConfig.Current);
            }

            return RedirectToAction("UserConfig");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserConfig(UserConfiguration config)
        {
            if (ModelState.IsValid && config != null)
            {
                userConfigRepository.SaveConfig(config);
                UserConfiguration.Current = userConfigRepository.ReadConfig();
            }
            return RedirectToAction("UserConfig");
        }

        public ActionResult CustomFields(string type)
        {
            if (string.IsNullOrEmpty(type) || !(new[] { "buyer", "seller", "quote", "product", "portfolio", "cartitem" }.Contains(type.ToLower())))
                return HttpNotFound();

            CustomFieldEntityType entityType;

            switch (type.ToLower())
            {
                case "buyer":
                    entityType = CustomFieldEntityType.Buyer;
                    break;

                case "seller":
                    entityType = CustomFieldEntityType.Seller;
                    break;

                case "quote":
                    entityType = CustomFieldEntityType.Quote;
                    break;

                case "product":
                    entityType = CustomFieldEntityType.Product;
                    break;

                case "portfolio":
                    entityType = CustomFieldEntityType.Portfolio;
                    break;

                case "cartitem":
                    entityType = CustomFieldEntityType.CartItem;
                    break;
                default:
                    return HttpNotFound();
            }

            if (!MaaSConfig.Current.CustomFields.ForEntity(entityType).Enabled)
                return HttpNotFound();

            var model = new CustomizeCustomFieldsVM()
            {
                EntityType = entityType,
                FieldDefinitions = customFieldRepository.ListByEntityType(entityType).Select(x => new CustomFieldVM(x)).OrderBy(x => x.Order).ToList(),
            };

            return View("CustomFields", model);
        }

        [HttpPost]
        public ActionResult NewCustomField(CustomFieldVM field)
        {
            if (ModelState.IsValid)
            {
                if (!field.DataType.HasValue)
                    ModelState.AddErrors(OperationResult.Fail("Campo obrigatório não preenchido"));

                var allfields = customFieldRepository.ListByEntityType(field.EntityType);
                if (allfields.Count >= MaaSConfig.Current.CustomFields.ForEntity(field.EntityType).MaxCustomFields)
                {
                    ModelState.AddErrors(OperationResult.Fail("Limite máximo de campos atingido"));
                }
                else
                {
                    SaveNewCustomField(field);
                }
            }

            return RedirectToAction("CustomFields", new { type = field.EntityType.ToString().ToLower() });
        }

        [HttpPost]
        public ActionResult UpdateField(CustomFieldVM field)
        {
            if (ModelState.IsValid)
            {
                UpdateCustomField(field);
            }

            return RedirectToAction("CustomFields", new { type = field.EntityType.ToString().ToLower() });
        }

        [HttpPost]
        public ActionResult DeleteCustomField(string confirmed, string fieldId, string entityType)
        {
            if (confirmed != null && confirmed == "true")
            {
                customFieldRepository.Delete(new Guid(fieldId));
            }

            return RedirectToAction("CustomFields", new { type = entityType });
        }

        [HttpPost]
        public ActionResult AddOptionToField(string fieldId, string option, string entityType)
        {
            if (string.IsNullOrWhiteSpace(option))
            {
                return HttpNotFound();
            }

            var field = customFieldRepository.GetById(fieldId);
            field.Options.Add(new CustomFieldOption() { Name = option });
            customFieldRepository.Update(field);

            var model = new CustomFieldVM(field);

            return PartialView("~/Areas/Admin/Views/NewCustomize/_CustomFieldOptions.cshtml", model);
        }

        [HttpPost]
        public ActionResult RemoveOptionFromField(string fieldId, string optionId, string entityType)
        {
            var field = customFieldRepository.GetById(fieldId);

            var option = field.Options.FirstOrDefault(x => x.Id.ToString() == optionId);
            if (option != null)
            {
                customFieldRepository.DeleteOption(field, option);
            }

            var model = new CustomFieldVM(field);
            return PartialView("~/Areas/Admin/Views/NewCustomize/_CustomFieldOptions.cshtml", model);
        }

        [HttpGet]
        public ActionResult Quote(string lang = "pt-br")
        {
            return View("Quote", new CustomizeQuoteRequestVM(lang, MaaSConfig.Current));
        }

        [HttpPost]
        public ActionResult Quote(CustomizeQuoteRequestVM model)
        {
            if (ModelState.IsValid)
            {
                GlobalizationHelper.UpdateString(stringRepository, "CITY_AND_STATE_LABEL", model.Lang, model.CityAndState.Title);
                GlobalizationHelper.UpdateString(stringRepository, "ADDRESS_INPUT", model.Lang, model.Address.Title);
                GlobalizationHelper.UpdateString(stringRepository, "QUOTE_DATE_LABEL", model.Lang, model.Date.Title);
                GlobalizationHelper.UpdateString(stringRepository, "PHONE_LABEL", model.Lang, model.Phone.Title);
                GlobalizationHelper.UpdateString(stringRepository, "QUOTE_PREFERENCE_LABEL", model.Lang, model.Preference.Title);
                GlobalizationHelper.UpdateString(stringRepository, "QUOTE_MAX_BUDGET_LABEL", model.Lang, model.QuoteMax.Title);

                model.CopyTo(MaaSConfig.Current);
                SaveConfig(MaaSConfig.Current);
                return RedirectToAction("Quote");
            }

            else return View("Quote", model);
        }

        [HttpGet]
        public ActionResult Payment()
        {
            if (!MaaSConfig.Current.Payment.Enabled || (MaaSConfig.Current.Payment.ActiveGateway != "moip" && MaaSConfig.Current.Payment.ActiveGateway != "pagarme"))
                return HttpNotFound();

            var model = new BusinessConfigViewModel(MaaSConfig.Current.Payment.ActiveGateway);

            return View("Payment", model);
        }

        [HttpPost]
        public ActionResult Payment(BusinessConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.SaveToConfig(MaaSConfig.Current);
                SaveConfig(MaaSConfig.Current);
                return RedirectToAction("Others");
            }

            return View(model);
        }

        #region HelperMethods

        /// <summary>
        /// Validates a URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool ValidateUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri validatedUri)) //.NET URI validation.
            {
                //If true: validatedUri contains a valid Uri. Check for the scheme in addition.
                return (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps);
            }
            return false;
        }

        public string GetInputOptions(string fieldType)
        {
            if (string.IsNullOrWhiteSpace(fieldType))
                return "";

            List<CustomFieldInputType> inputTypes = CustomFieldVM.InputOptionsForDataType((CustomFieldDataType)(Enum.Parse(typeof(CustomFieldDataType), fieldType)));
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var inp in inputTypes)
            {
                stringBuilder.Append($@"<option value=""{inp.ToString()}"">{GlobalizationHelper.GetString($"CUSTOM_FIELD_INPUTTYPE_{inp.ToString().ToUpperInvariant()}")}</option>");
            }

            return stringBuilder.ToString();
        }

        private void ClearDonutCache()
        {
            var cacheManager = new OutputCacheManager();
            cacheManager.RemoveItems();
        }

        private void SetErrors(ErrorDictionary errors)
        {
            if (errors != null && errors.Count > 0)
                HttpContext.Items.Add(errorsKey, errors);
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

                    default: return new OperationResult("", "Invalid CustomizeOption value: " + option.ToString());
                }

                SaveConfig(config);
            }

            return OperationResult.Success;
        }

        private void SaveConfig(MaaSConfig config)
        {
            var appConfig = appConfigRepository.Query().FirstOrDefault();
            if (appConfig == null) appConfig = new ApplicationConfig();

            var oldConfig = appConfig.GetConfig();

            appConfig.MaaSConfig = config.ToJson();

            if (appConfig.Id == 0) appConfigRepository.Insert(appConfig);
            else appConfigRepository.Update(appConfig);

            appConfigRepository.SaveChanges();

            MaaSConfig.Load(appConfig);
            _domainEventDispatcher.Dispatch(new ConfigUpdatedEvent(MaaSConfig.Current, oldConfig));
        }

        private CustomizeHomeVM GetHomeVM(string lang)
        {
            string queryLabel = "";

            var config = MaaSConfig.Current;

            switch (config.Search.SearchType)
            {
                case Domain.Config.General.SearchType.Seller:
                    queryLabel = "SELLER_FILTER_INPUT";
                    break;

                case Domain.Config.General.SearchType.Product:
                    queryLabel = "PRODUCT_FILTER_INPUT";
                    break;

                case Domain.Config.General.SearchType.Portfolio:
                    queryLabel = "PORTFOLIO_FILTER_INPUT";
                    break;
            }

            var model = new CustomizeHomeVM();

            model.Lang = lang;

            model.HomeHero = new CustomizeHomeHeroVM()
            {
                Slogan1 = GlobalizationHelper.GetString("APPLICATION_SLOGAN_1", lang),
                Slogan2 = GlobalizationHelper.GetString("APPLICATION_SLOGAN_2", lang),
                HomeBackgroundUrl = config.Layout.Images.Background1,

                SearchCategory = new CustomizeFieldVM()
                {
                    Placeholder = GlobalizationHelper.GetString("CATEGORY_SELECT", lang),
                    Status = config.Search.SearchFields.Category ? CustomizeFieldVM.FieldStatus.ENABLED_OPTIONAL : CustomizeFieldVM.FieldStatus.DISABLED,
                },
                SearchLocation = new CustomizeFieldVM()
                {
                    Status = config.Search.SearchFields.Location ? CustomizeFieldVM.FieldStatus.ENABLED_OPTIONAL : CustomizeFieldVM.FieldStatus.DISABLED,
                },
                SearchQuery = new CustomizeFieldVM()
                {
                    Placeholder = GlobalizationHelper.GetString(queryLabel, lang),
                    Status = config.Search.SearchFields.Query ? CustomizeFieldVM.FieldStatus.ENABLED_OPTIONAL : CustomizeFieldVM.FieldStatus.DISABLED,
                },
                SearchRange = config.Search.Map.SearchRange,
                SearchType = config.Search.SearchType,
            };

            model.HowItWorks = new CustomizeHowItWorksVM()
            {
                SectionTitle = GlobalizationHelper.GetString("HOW_IT_WORKS_TITLE", lang),
                SectionEnabled = config.HowItWorks.Enabled,
                Item1 = new CustomizeHomeIconVM()
                {
                    IconUrl = config.HowItWorks.Items[0].ImageUrl,
                    Description = GlobalizationHelper.GetString("HOW_IT_WORKS_TEXT_1", lang),
                    Title = GlobalizationHelper.GetString("HOW_IT_WORKS_TITLE_1", lang),
                },
                Item2 = new CustomizeHomeIconVM()
                {
                    IconUrl = config.HowItWorks.Items[1].ImageUrl,
                    Description = GlobalizationHelper.GetString("HOW_IT_WORKS_TEXT_2", lang),
                    Title = GlobalizationHelper.GetString("HOW_IT_WORKS_TITLE_2", lang),
                },
                Item3 = new CustomizeHomeIconVM()
                {
                    IconUrl = config.HowItWorks.Items[2].ImageUrl,
                    Description = GlobalizationHelper.GetString("HOW_IT_WORKS_TEXT_3", lang),
                    Title = GlobalizationHelper.GetString("HOW_IT_WORKS_TITLE_3", lang),
                },
            };

            model.BestSellersTitle = GlobalizationHelper.GetString("BEST_SELLERS_TITLE", lang);
            model.EnableBestSellers = config.BestSellers.Enabled;

            model.SellerCall = new CustomizeSellerCallVM()
            {
                ButtonText = GlobalizationHelper.GetString("CALL_SELLERS_LINK", lang),
                Text1 = GlobalizationHelper.GetString("HELP_QUESTION_1", lang),
                Text2 = GlobalizationHelper.GetString("HELP_QUESTION_2", lang),
                Text3 = GlobalizationHelper.GetString("HELP_QUESTION_3", lang),
                Title = GlobalizationHelper.GetString("CALL_SELLERS_TITLE", lang),
                Enabled = config.HomeCallSellers.Enabled,
                IconUrl = config.Layout.Images.HomeCallSellers,
            };

            model.MiscArea = new CustomizeMiscAreaVM()
            {
                EnableSection = config.MiscArea.Enabled,
                SectionTitle = GlobalizationHelper.GetString("MISC_AREA_TITLE", lang),
                Items = config.MiscArea.Items.Select(x => x.ImageUrl).ToList(),
            };

            return model;
        }

        private void SaveHomeVM(CustomizeHomeVM model, string lang)
        {
            string queryLabel = "";

            var config = MaaSConfig.Current;

            switch (config.Search.SearchType)
            {
                case Domain.Config.General.SearchType.Seller:
                    queryLabel = "SELLER_FILTER_INPUT";
                    break;

                case Domain.Config.General.SearchType.Product:
                    queryLabel = "PRODUCT_FILTER_INPUT";
                    break;

                case Domain.Config.General.SearchType.Portfolio:
                    queryLabel = "PORTFOLIO_FILTER_INPUT";
                    break;
            }

            GlobalizationHelper.UpdateString(stringRepository, "APPLICATION_SLOGAN_1", lang, model.HomeHero.Slogan1);
            GlobalizationHelper.UpdateString(stringRepository, "APPLICATION_SLOGAN_2", lang, model.HomeHero.Slogan2);
            config.Layout.Images.Background1 = model.HomeHero.HomeBackgroundUrl;

            GlobalizationHelper.UpdateString(stringRepository, "CATEGORY_SELECT", lang, model.HomeHero.SearchCategory.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, queryLabel, lang, model.HomeHero.SearchQuery.Placeholder);
            config.Search.SearchFields.Category = model.HomeHero.SearchCategory.Status != CustomizeFieldVM.FieldStatus.DISABLED;
            config.Search.SearchFields.Location = model.HomeHero.SearchLocation.Status != CustomizeFieldVM.FieldStatus.DISABLED;
            config.Search.SearchFields.Query = model.HomeHero.SearchQuery.Status != CustomizeFieldVM.FieldStatus.DISABLED;
            config.Search.Map.SearchRange = model.HomeHero.SearchRange;
            config.Search.SearchType = model.HomeHero.SearchType;
            config.Search.ResultType = (Domain.Config.General.ResultType)((int)(model.HomeHero.SearchType));

            config.HowItWorks.Enabled = model.HowItWorks.SectionEnabled;

            GlobalizationHelper.UpdateString(stringRepository, "HOW_IT_WORKS_TITLE", lang, model.HowItWorks.SectionTitle);

            GlobalizationHelper.UpdateString(stringRepository, "HOW_IT_WORKS_TEXT_1", lang, model.HowItWorks.Item1.Description);
            GlobalizationHelper.UpdateString(stringRepository, "HOW_IT_WORKS_TITLE_1", lang, model.HowItWorks.Item1.Title);
            config.HowItWorks.Items[0].ImageUrl = model.HowItWorks.Item1.IconUrl;

            GlobalizationHelper.UpdateString(stringRepository, "HOW_IT_WORKS_TEXT_2", lang, model.HowItWorks.Item2.Description);
            GlobalizationHelper.UpdateString(stringRepository, "HOW_IT_WORKS_TITLE_2", lang, model.HowItWorks.Item2.Title);
            config.HowItWorks.Items[1].ImageUrl = model.HowItWorks.Item2.IconUrl;

            GlobalizationHelper.UpdateString(stringRepository, "HOW_IT_WORKS_TEXT_3", lang, model.HowItWorks.Item3.Description);
            GlobalizationHelper.UpdateString(stringRepository, "HOW_IT_WORKS_TITLE_3", lang, model.HowItWorks.Item3.Title);
            config.HowItWorks.Items[2].ImageUrl = model.HowItWorks.Item3.IconUrl;

            GlobalizationHelper.UpdateString(stringRepository, "BEST_SELLERS_TITLE", lang, model.BestSellersTitle);
            config.BestSellers.Enabled = model.EnableBestSellers;

            GlobalizationHelper.UpdateString(stringRepository, "CALL_SELLERS_LINK", lang, model.SellerCall.ButtonText);
            GlobalizationHelper.UpdateString(stringRepository, "HELP_QUESTION_1", lang, model.SellerCall.Text1);
            GlobalizationHelper.UpdateString(stringRepository, "HELP_QUESTION_2", lang, model.SellerCall.Text2);
            GlobalizationHelper.UpdateString(stringRepository, "HELP_QUESTION_3", lang, model.SellerCall.Text3);
            GlobalizationHelper.UpdateString(stringRepository, "CALL_SELLERS_TITLE", lang, model.SellerCall.Title);
            config.HomeCallSellers.Enabled = model.SellerCall.Enabled;
            config.Layout.Images.HomeCallSellers = model.SellerCall.IconUrl;

            GlobalizationHelper.UpdateString(stringRepository, "MISC_AREA_TITLE", lang, model.MiscArea.SectionTitle);
            config.MiscArea.Enabled = model.MiscArea.EnableSection;
            config.MiscArea.Items = model.MiscArea.Items.Select(x => new Application.Config.Sections.MiscAreaItemConfig() { ImageUrl = x }).ToList();

            SaveConfig(config);
        }

        private CustomizeSellerRegisterVM GetCustomizeSellerVM(string lang)
        {
            var seller = MaaSConfig.Current.Seller;

            var viewModel = new CustomizeSellerRegisterVM()
            {
                Lang = lang,
                AboutMe = new CustomizeFieldVM(lang, "ABOUT_ME_INPUT", "ABOUT_ME_LABEL", seller.AboutMe.Enabled, seller.AboutMe.Mandatory),
                AdditionalInfo1 = new CustomizeFieldVM(lang, "ADDITIONAL_INFO_1_INPUT", "ADDITIONAL_INFO_1_LABEL", seller.AdditionalInfo1),
                AdditionalInfo2 = new CustomizeFieldVM(lang, "ADDITIONAL_INFO_2_INPUT", "ADDITIONAL_INFO_2_LABEL", seller.AdditionalInfo2),
                AlternateName = new CustomizeFieldVM(lang, "ALTERNATIVE_INPUT", "ALTERNATIVE_LABEL", seller.AlternativeName),
                CityAndState = new CustomizeFieldVM(lang, "CITY_AND_STATE_INPUT", "CITY_AND_STATE_LABEL", seller.CityAndState.Enabled, seller.CityAndState.Mandatory),
                DateOfBirth = new CustomizeFieldVM(lang, "DATE_OF_BIRTH_INPUT", "DATE_OF_BIRTH_LABEL", seller.DateOfBirth, blockChangesAfterApprovedInModeration: seller.DateOfBirth.BlockChangesAfterApprovedInModeration),
                DocumentNumber = new CustomizeFieldVM(lang, "DOCUMENT_NUMBER_INPUT", "DOCUMENT_NUMBER_LABEL", seller.DocumentNumber, blockChangesAfterApprovedInModeration: seller.DocumentNumber.BlockChangesAfterApprovedInModeration),
                FacebookProfile = new CustomizeFieldVM(lang, "", "", seller.FacebookProfile),
                Gender = new CustomizeFieldVM(lang, "GENRE_INPUT", "GENRE_LABEL", seller.Genre),
                InstagramProfile = new CustomizeFieldVM(lang, "", "", seller.InstagramProfile),
                LinkedInProfile = new CustomizeFieldVM(lang, "", "", seller.LinkedInProfile),
                WhatsAppNumber = new CustomizeFieldVM(lang, "", "", seller.WhatsAppNumber),
                Phone = new CustomizeFieldVM(lang, "PHONE_INPUT", "PHONE_LABEL", seller.Phone),
                HelpIconUrl1 = MaaSConfig.Current.HelpQuestions.Items[0].ImageUrl,
                HelpIconUrl2 = MaaSConfig.Current.HelpQuestions.Items[1].ImageUrl,
                HelpIconUrl3 = MaaSConfig.Current.HelpQuestions.Items[2].ImageUrl,
            };

            return viewModel;
        }

        private void SaveCustomizeSellerVM(CustomizeSellerRegisterVM model)
        {
            GlobalizationHelper.UpdateString(stringRepository, "ABOUT_ME_INPUT", model.Lang, model.AboutMe.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "ABOUT_ME_LABEL", model.Lang, model.AboutMe.Title);
            GlobalizationHelper.UpdateString(stringRepository, "ADDITIONAL_INFO_1_INPUT", model.Lang, model.AdditionalInfo1.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "ADDITIONAL_INFO_1_LABEL", model.Lang, model.AdditionalInfo1.Title);
            GlobalizationHelper.UpdateString(stringRepository, "ADDITIONAL_INFO_2_INPUT", model.Lang, model.AdditionalInfo2.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "ADDITIONAL_INFO_2_LABEL", model.Lang, model.AdditionalInfo2.Title);
            GlobalizationHelper.UpdateString(stringRepository, "ALTERNATIVE_INPUT", model.Lang, model.AlternateName.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "ALTERNATIVE_LABEL", model.Lang, model.AlternateName.Title);
            //GlobalizationHelper.UpdateString(_stringRepository, "CITY_AND_STATE_INPUT", model.Lang, model.CityAndState.Placeholder);
            //GlobalizationHelper.UpdateString(_stringRepository, "CITY_AND_STATE_LABEL", model.Lang, model.CityAndState.Title);
            GlobalizationHelper.UpdateString(stringRepository, "DATE_OF_BIRTH_INPUT", model.Lang, model.DateOfBirth.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "DATE_OF_BIRTH_LABEL", model.Lang, model.DateOfBirth.Title);
            GlobalizationHelper.UpdateString(stringRepository, "DOCUMENT_NUMBER_INPUT", model.Lang, model.DocumentNumber.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "DOCUMENT_NUMBER_LABEL", model.Lang, model.DocumentNumber.Title);
            GlobalizationHelper.UpdateString(stringRepository, "GENRE_INPUT", model.Lang, model.Gender.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "GENRE_LABEL", model.Lang, model.Gender.Title);
            GlobalizationHelper.UpdateString(stringRepository, "PHONE_INPUT", model.Lang, model.Phone.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "PHONE_LABEL", model.Lang, model.Phone.Title);

            var config = MaaSConfig.Current.Seller;

            config.AboutMe.Enabled = model.AboutMe.IsEnabled;
            config.AboutMe.Mandatory = model.AboutMe.IsMandatory;

            config.AdditionalInfo1.Enabled = model.AdditionalInfo1.IsEnabled;
            config.AdditionalInfo1.Mandatory = model.AdditionalInfo1.IsMandatory;

            config.AdditionalInfo2.Enabled = model.AdditionalInfo2.IsEnabled;
            config.AdditionalInfo2.Mandatory = model.AdditionalInfo2.IsMandatory;

            config.AlternativeName.Enabled = model.AlternateName.IsEnabled;
            config.AlternativeName.Mandatory = model.AlternateName.IsMandatory;

            //config.CityAndState.Enabled = model.CityAndState.IsEnabled;
            //config.CityAndState.Mandatory = model.CityAndState.IsMandatory;

            config.DateOfBirth.Enabled = model.DateOfBirth.IsEnabled;
            config.DateOfBirth.Mandatory = model.DateOfBirth.IsMandatory;
            config.DateOfBirth.BlockChangesAfterApprovedInModeration = model.DateOfBirth.BlockChangesAfterApprovedInModeration;

            config.DocumentNumber.Enabled = model.DocumentNumber.IsEnabled;
            config.DocumentNumber.Mandatory = model.DocumentNumber.IsMandatory;
            config.DocumentNumber.BlockChangesAfterApprovedInModeration = model.DocumentNumber.BlockChangesAfterApprovedInModeration;

            config.Genre.Enabled = model.Gender.IsEnabled;
            config.Genre.Mandatory = model.Gender.IsMandatory;

            config.Phone.Enabled = model.Phone.IsEnabled;
            config.Phone.Mandatory = model.Phone.IsMandatory;

            model.FacebookProfile.CopyTo(config.FacebookProfile);
            model.InstagramProfile.CopyTo(config.InstagramProfile);
            model.LinkedInProfile.CopyTo(config.LinkedInProfile);
            model.WhatsAppNumber.CopyTo(config.WhatsAppNumber);

            MaaSConfig.Current.HelpQuestions.Items[0].ImageUrl = model.HelpIconUrl1;
            MaaSConfig.Current.HelpQuestions.Items[1].ImageUrl = model.HelpIconUrl2;
            MaaSConfig.Current.HelpQuestions.Items[2].ImageUrl = model.HelpIconUrl3;

            SaveConfig(MaaSConfig.Current);
        }

        private CustomizeBuyerRegisterVM GetCustomizeBuyerVM(string lang)
        {
            var buyer = MaaSConfig.Current.Buyer;

            return new CustomizeBuyerRegisterVM()
            {
                Lang = lang,
                Address = new CustomizeFieldVM(lang, "", "", buyer.Address.Enabled, buyer.Address.Mandatory),
                Avatar = buyer.Avatar.Enabled,
                CityAndState = new CustomizeFieldVM(lang, "CITY_AND_STATE_INPUT", "CITY_AND_STATE_LABEL", buyer.CityAndState.Enabled, buyer.CityAndState.Mandatory),
                DateOfBirth = new CustomizeFieldVM(lang, "DATE_OF_BIRTH_INPUT", "DATE_OF_BIRTH_LABEL", buyer.DateOfBirth.Enabled, buyer.DateOfBirth.Mandatory,blockChangesAfterApprovedInModeration: buyer.DateOfBirth.BlockChangesAfterApprovedInModeration),
                DocumentNumber = new CustomizeFieldVM(lang, "DOCUMENT_NUMBER_INPUT_BUYER", "DOCUMENT_NUMBER_LABEL_BUYER", buyer.DocumentNumber.Enabled, buyer.DocumentNumber.Mandatory,blockChangesAfterApprovedInModeration: buyer.DocumentNumber.BlockChangesAfterApprovedInModeration),
                Gender = new CustomizeFieldVM(lang, "GENRE_INPUT", "GENRE_LABEL", buyer.Genre.Enabled, buyer.Genre.Mandatory),
                Phone = new CustomizeFieldVM(lang, "PHONE_INPUT", "PHONE_LABEL", buyer.Phone.Enabled, buyer.Phone.Mandatory),
            };
        }

        private void SaveBuyerRegisterVM(CustomizeBuyerRegisterVM model)
        {
            var buyer = MaaSConfig.Current.Buyer;

            buyer.Address.Enabled = model.Address.IsEnabled;
            buyer.Address.Mandatory = model.Address.IsMandatory;

            buyer.Avatar.Enabled = model.Avatar;

            buyer.CityAndState.Enabled = model.CityAndState.IsEnabled;
            buyer.CityAndState.Mandatory = model.CityAndState.IsMandatory;
            GlobalizationHelper.UpdateString(stringRepository, "CITY_AND_STATE_INPUT", model.Lang, model.CityAndState.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "CITY_AND_STATE_LABEL", model.Lang, model.CityAndState.Title);

            buyer.DateOfBirth.Enabled = model.DateOfBirth.IsEnabled;
            buyer.DateOfBirth.Mandatory = model.DateOfBirth.IsMandatory;
            buyer.DateOfBirth.BlockChangesAfterApprovedInModeration = model.DateOfBirth.BlockChangesAfterApprovedInModeration;
            GlobalizationHelper.UpdateString(stringRepository, "DATE_OF_BIRTH_INPUT", model.Lang, model.DateOfBirth.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "DATE_OF_BIRTH_LABEL", model.Lang, model.DateOfBirth.Title);

            buyer.DocumentNumber.Enabled = model.DocumentNumber.IsEnabled;
            buyer.DocumentNumber.Mandatory = model.DocumentNumber.IsMandatory;
            buyer.DocumentNumber.BlockChangesAfterApprovedInModeration = model.DocumentNumber.BlockChangesAfterApprovedInModeration;
            GlobalizationHelper.UpdateString(stringRepository, "DOCUMENT_NUMBER_INPUT_BUYER", model.Lang, model.DocumentNumber.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "DOCUMENT_NUMBER_LABEL_BUYER", model.Lang, model.DocumentNumber.Title);

            buyer.Genre.Enabled = model.Gender.IsEnabled;
            buyer.Genre.Mandatory = model.Gender.IsMandatory;
            GlobalizationHelper.UpdateString(stringRepository, "GENRE_INPUT", model.Lang, model.Gender.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "GENRE_LABEL", model.Lang, model.Gender.Title);

            buyer.Phone.Enabled = model.Phone.IsEnabled;
            buyer.Phone.Mandatory = model.Phone.IsMandatory;
            GlobalizationHelper.UpdateString(stringRepository, "PHONE_INPUT", model.Lang, model.Phone.Placeholder);
            GlobalizationHelper.UpdateString(stringRepository, "PHONE_LABEL", model.Lang, model.Phone.Title);

            SaveConfig(MaaSConfig.Current);
        }

        private void SaveNewCustomField(CustomFieldVM model)
        {
            CustomField field = new CustomField(model.DataType.Value, model.EntityType, model.InputType)
            {
                IsMandatory = model.IsMandatory,
                IsPublic = model.IsPublic,
                Name = model.Name,
                Name_EN = model.Name_EN,
                Order = model.Order,
                Placeholder = model.Placeholder,
                Placeholder_EN = model.Placeholder_EN,
                BlockChangesAfterApprovedInModeration = model.BlockChangesAfterApprovedInModeration
            };

            customFieldRepository.Add(field);
        }

        private void UpdateCustomField(CustomFieldVM model)
        {
            var field = customFieldRepository.GetById(model.Id);

            field.IsMandatory = model.IsMandatory;
            field.IsPublic = model.IsPublic;
            field.Name = model.Name;
            field.Name_EN = model.Name_EN;
            field.Order = model.Order;
            field.Placeholder = model.Placeholder;
            field.Placeholder_EN = model.Placeholder_EN;
            field.BlockChangesAfterApprovedInModeration = model.BlockChangesAfterApprovedInModeration;

            customFieldRepository.Update(field);
        }

        private string ExtractEmail(string s)
        {
            Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

            if (regex.IsMatch(s))
            {
                return regex.Match(s).Value;
            }

            return null;
        }

        #endregion HelperMethods
    }
}