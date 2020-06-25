using Application.Config;
using Application.Config.Common;
using Domain.Config.General;
using Domain.Entities;
using Domain.UserConfig;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Models
{
    public class CustomizeMarketplaceViewModels
    {
    }

    public class CustomizeMarketplaceVM
    {
        public string Lang { get; set; } = MaaSConfig.Current.Localization.DefaultLang;

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_MARKETPLACE_NAME_LABEL")]
        public string MarketplaceName { get; set; }

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_MARKETPLACE_DOC_LABEL")]
        public string DocumentNumber { get; set; }

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_MARKETPLACE_EMAIL_LABEL")]
        public string CustomerServiceEmail { get; set; }

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_MARKETPLACE_ABOUT_LABEL")]
        public string AboutText { get; set; }
    }

    #region Home

    public class CustomizeHomeVM
    {
        public string Lang { get; set; } = MaaSConfig.Current.Localization.DefaultLang;

        public string BestSellersTitle { get; set; }
        public bool EnableBestSellers { get; set; }

        public CustomizeHomeHeroVM HomeHero { get; set; } = new CustomizeHomeHeroVM();
        public CustomizeHowItWorksVM HowItWorks { get; set; } = new CustomizeHowItWorksVM();
        public CustomizeSellerCallVM SellerCall { get; set; } = new CustomizeSellerCallVM();
        public CustomizeMiscAreaVM MiscArea { get; set; } = new CustomizeMiscAreaVM();
    }

    public class CustomizeHomeHeroVM
    {
        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_HOME_SLOGAN1_LABEL")]
        public string Slogan1 { get; set; }

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_HOME_SLOGAN2_LABEL")]
        public string Slogan2 { get; set; }

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_HOME_SEARCHTYPE_LABEL")]
        public SearchType SearchType { get; set; }

        [DisplayNameLocalized("ADMIN_CUSTOMIZE_HOME_SEARCHQUERY_LABEL")]
        public CustomizeFieldVM SearchQuery { get; set; } = new CustomizeFieldVM();

        [DisplayNameLocalized("ADMIN_CUSTOMIZE_HOME_SEARCHCATEGORY_LABEL")]
        public CustomizeFieldVM SearchCategory { get; set; } = new CustomizeFieldVM();

        [DisplayNameLocalized("ADMIN_CUSTOMIZE_HOME_SEARCHLOCATION_LABEL")]
        public CustomizeFieldVM SearchLocation { get; set; } = new CustomizeFieldVM();

        public bool LocationSearchEnabled => SearchLocation.Status != CustomizeFieldVM.FieldStatus.DISABLED;

        public string HomeBackgroundUrl { get; set; }

        [DisplayNameLocalized("ADMIN_CUSTOMIZE_HOME_SEARCHRANGE_LABEL")]
        [RequiredIfLocalized("LocationSearchEnabled")]
        public int SearchRange { get; set; }
    }

    public class CustomizeHowItWorksVM
    {
        public bool SectionEnabled { get; set; }
        public string SectionTitle { get; set; }
        public CustomizeHomeIconVM Item1 { get; set; } = new CustomizeHomeIconVM();
        public CustomizeHomeIconVM Item2 { get; set; } = new CustomizeHomeIconVM();
        public CustomizeHomeIconVM Item3 { get; set; } = new CustomizeHomeIconVM();
    }

    public class CustomizeHomeIconVM
    {
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_ICON")]
        public string IconUrl { get; set; }

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_TITLE")]
        public string Title { get; set; }

        [Required]
        [DisplayNameLocalized("ADMIN_CUSTOMIZE_DESCRIPTION")]
        public string Description { get; set; }
    }

    public class CustomizeSellerCallVM
    {
        public bool Enabled { get; set; }

        public string IconUrl { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ButtonText { get; set; }

        [Required]
        public string Text1 { get; set; }

        [Required]
        public string Text2 { get; set; }

        [Required]
        public string Text3 { get; set; }
    }

    public class CustomizeMiscAreaVM
    {
        public string SectionTitle { get; set; }
        public bool EnableSection { get; set; }

        public List<string> Items { get; set; } = new List<string>();
    }

    #endregion Home

    #region Identity

    public class CustomizeIdentityVM
    {
        public CustomizeIdentityColorPresetVM CurrentPreset { get; set; } = new CustomizeIdentityColorPresetVM();

        public CustomizeIdentityColorPresetVM Preset1 { get; set; } = new CustomizeIdentityColorPresetVM()
        {
            Accent1 = "#3498DB",
            Accent2 = "#2C3E50",
            Contrast = "#E74C3C",
            MenuHomeBG = "rgba(0,0,0,0)",
            MenuHomeColorAccent = "#FFFFFF",
        };

        public CustomizeIdentityColorPresetVM Preset2 { get; set; } = new CustomizeIdentityColorPresetVM()
        {
            Accent1 = "#7286A0",
            Accent2 = "#A3BFA8",
            Contrast = "#BE6E46",
            MenuHomeBG = "rgba(0,0,0,0)",
            MenuHomeColorAccent = "#FFFFFF",
        };

        public CustomizeIdentityColorPresetVM Preset3 { get; set; } = new CustomizeIdentityColorPresetVM()
        {
            Accent1 = "#44355B",
            Accent2 = "#31263E",
            Contrast = "#ECA72C",
            MenuHomeBG = "#221E22",
            MenuHomeColorAccent = "#FFFFFF",
        };

        public CustomizeIdentityColorPresetVM Preset4 { get; set; } = new CustomizeIdentityColorPresetVM()
        {
            Accent1 = "#3D0814",
            Accent2 = "#442F38",
            Contrast = "#C6B38E",
            MenuHomeBG = "rgba(0,0,0,0)",
            MenuHomeColorAccent = "#E7F9A9",
        };

        public CustomizeIdentityColorPresetVM Preset5 { get; set; } = new CustomizeIdentityColorPresetVM()
        {
            Accent1 = "#DEB841",
            Accent2 = "#DE9E36",
            Contrast = "#37323E",
            MenuHomeBG = "rgba(0,0,0,0)",
            MenuHomeColorAccent = "#FFFFFF",
        };

        public CustomizeIdentityColorPresetVM Preset6 { get; set; } = new CustomizeIdentityColorPresetVM()
        {
            Accent1 = "#3498DB",
            Accent2 = "#2C3E50",
            Contrast = "#E74C3C",
            MenuHomeBG = "rgba(0,0,0,0)",
            MenuHomeColorAccent = "#FFFFFF",
        };
    }

    public class CustomizeIdentityColorPresetVM
    {
        public string Accent1 { get; set; } = "#FF00FF";

        public string Accent2 { get; set; } = "#00FFFF";

        public string Contrast { get; set; } = "#FFFF00";

        public string MenuHomeBG { get; set; } = "#00FF00";

        public string MenuHomeColorAccent { get; set; } = "#0000FF";

        public void SanitizeColors()
        {
            Accent1 = SanitizeColor(Accent1);
            Accent2 = SanitizeColor(Accent2);
            Contrast = SanitizeColor(Contrast);
            MenuHomeBG = SanitizeColor(MenuHomeBG);
            MenuHomeColorAccent = SanitizeColor(MenuHomeColorAccent);
        }

        private string SanitizeColor(string color)
        {
            Regex isHex = new Regex(@"^#[a-fA-F0-9]{6}$");
            Regex isRgb = new Regex(@"^rgb\((\d{1,3}),\s?(\d{1,3}),\s?(\d{1,3})\)$");
            Regex isRgba = new Regex(@"^rgba\((\d{1,3}),(\s?\d{1,3}),(\s?\d{1,3}),((0|1)(.\d+)?)\)$");
            Regex isOtherColor = new Regex(@"^\$[A-z0-9_]+$");

            if (color == "transparent")
                return "rgba(0,0,0,0)";

            if (isHex.IsMatch(color))
                return color;

            if (isRgb.IsMatch(color))
            {
                var match = isRgb.Match(color);

                string R = int.Parse(match.Groups[1].Value).ToString("X2");
                string G = int.Parse(match.Groups[2].Value).ToString("X2");
                string B = int.Parse(match.Groups[3].Value).ToString("X2");

                return $"#{R}{G}{B}";
            }

            if (isRgba.IsMatch(color))
                return color;

            if (isOtherColor.IsMatch(color))
                return SanitizeColor(MaaSConfig.Current.Layout.Colors[color]);

            return "#FFFFFF";
        }
    }

    #endregion Identity

    #region Shared

    public class CustomizeFieldVM
    {
        public string Title { get; set; }

        public string Placeholder { get; set; }

        [Required]
        public FieldStatus Status { get; set; }

        public bool IsEnabled => Status != FieldStatus.DISABLED;

        public bool IsMandatory => Status == FieldStatus.ENABLED_MANDATORY;

        public bool IsPublic { get; set; }

        public bool BlockChangesAfterApprovedInModeration { get; set; }

        public enum FieldStatus
        {
            DISABLED = 0,
            ENABLED_OPTIONAL = 1,
            ENABLED_MANDATORY = 2,
        }

        public CustomizeFieldVM()
        {
        }

        public CustomizeFieldVM(string lang, string placeholder, string title, bool enabled, bool mandatory = false, bool isPublic = true, bool blockChangesAfterApprovedInModeration = false)
        {
            Placeholder = GlobalizationHelper.GetString(placeholder, lang) ?? placeholder;
            Title = GlobalizationHelper.GetString(title, lang) ?? title;
            Status = enabled ? (mandatory ? FieldStatus.ENABLED_MANDATORY : FieldStatus.ENABLED_OPTIONAL) : FieldStatus.DISABLED;
            IsPublic = isPublic;
            BlockChangesAfterApprovedInModeration = blockChangesAfterApprovedInModeration;
        }

        public CustomizeFieldVM(string lang, string placeholder, string title, PublicProfileRegisterConfig config, bool blockChangesAfterApprovedInModeration = false)
            : this(lang, placeholder, title, config.Enabled, config.Mandatory, config.ShowInPublicProfile, blockChangesAfterApprovedInModeration)
        {
        }

        public void CopyTo(PublicProfileRegisterConfig config)
        {
            config.Enabled = IsEnabled;
            config.Mandatory = IsMandatory;
            config.ShowInPublicProfile = IsPublic;
        }
    }

    #endregion Shared

    public class CustomizeSellerRegisterVM
    {
        public string Lang { get; set; } = "pt-br";

        public string HelpIconUrl1 { get; set; }
        public string HelpIconUrl2 { get; set; }
        public string HelpIconUrl3 { get; set; }

        public CustomizeFieldVM AboutMe { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM AlternateName { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM CityAndState { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Phone { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Gender { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM DocumentNumber { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM DateOfBirth { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM AdditionalInfo1 { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM AdditionalInfo2 { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM FacebookProfile { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM LinkedInProfile { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM InstagramProfile { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM GooglePlusProfile { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM WhatsAppNumber { get; set; } = new CustomizeFieldVM();
    }

    public class CustomizeBuyerRegisterVM
    {
        public string Lang { get; set; } = "pt-br";

        public bool Avatar { get; set; }

        public CustomizeFieldVM Address { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Phone { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Gender { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM DocumentNumber { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM DateOfBirth { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM CityAndState { get; set; } = new CustomizeFieldVM();
    }

    public class CustomizeCustomFieldsVM
    {
        public CustomFieldEntityType EntityType { get; set; }

        public List<CustomFieldVM> FieldDefinitions { get; set; }
    }

    public class CustomFieldVM
    {
        [Required]
        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_NAME")]
        public string Name { get; set; }

        //[Required]
        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_NAME_EN")]
        public string Name_EN { get; set; }

        //[Required]
        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_PLACEHOLDER")]
        public string Placeholder { get; set; }

        //[Required]
        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_PLACEHOLDER_EN")]
        public string Placeholder_EN { get; set; }

        [Required]
        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_ORDER")]
        public int Order { get; set; }

        [Required]
        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_DATATYPE")]
        public CustomFieldDataType? DataType { get; set; }

        [Required]
        public CustomFieldEntityType EntityType { get; set; }

        [Required]
        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_INPUTTYPE")]
        public CustomFieldInputType InputType { get; set; }

        public List<CustomFieldOption> Options { get; set; } = new List<CustomFieldOption>();

        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_ISPUBLIC")]
        public bool IsPublic { get; set; }

        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_ISREQUIRED")]
        public bool IsMandatory { get; set; }

        [DisplayNameLocalized("CUSTOM_FIELD_LABEL_BLOCKCHANGES")]
        public bool BlockChangesAfterApprovedInModeration { get; set; }

        public string Id { get; set; }

        public CustomFieldVM()
        {
        }

        public CustomFieldVM(CustomFieldEntityType type)
        {
            EntityType = type;
        }

        public CustomFieldVM(CustomField customField)
        {
            Name = customField.Name;
            Name_EN = customField.Name_EN;
            Placeholder = customField.Placeholder;
            Placeholder_EN = customField.Placeholder_EN;
            Order = customField.Order;
            DataType = customField.DataType;
            EntityType = customField.EntityType;
            InputType = customField.InputType;
            Options = customField.Options.OrderBy(x => x.CreatedAt).ToList();
            IsPublic = customField.IsPublic;
            IsMandatory = customField.IsMandatory;
            BlockChangesAfterApprovedInModeration = customField.BlockChangesAfterApprovedInModeration;
            Id = customField.Id.ToString();
        }

        public List<SelectListItem> GetDataTypeList<T>()
        {
            var list = new List<SelectListItem>();

            var types = (T[])Enum.GetValues(typeof(T));

            foreach (var v in types)
            {
                list.Add(new SelectListItem() { Value = v.ToString(), Text = v.ToString() });
            }

            return list;
        }

        public SelectList OptionsAsSelectList()
        {
            var optList = Options.Select(x => x.Name).ToList();
            optList.Sort();
            var selectList = new SelectList(optList);

            return selectList;
        }

        public string NameLocalized(string lang)
        {
            if (string.Equals(lang, "en-us", StringComparison.OrdinalIgnoreCase))
                return Name_EN;

            return Name;
        }

        public string PlaceholderLocalized(string lang)
        {
            if (string.Equals(lang, "en-us", StringComparison.OrdinalIgnoreCase))
                return Placeholder_EN;

            return Placeholder;
        }

        public static List<CustomFieldInputType> InputOptionsForDataType(CustomFieldDataType type)
        {
            var list = new List<CustomFieldInputType>();

            switch (type)
            {
                case CustomFieldDataType.RawText:
                    list = new List<CustomFieldInputType>() { CustomFieldInputType.Text, CustomFieldInputType.MultilineText, CustomFieldInputType.Options };
                    break;

                case CustomFieldDataType.Number:
                    list = new List<CustomFieldInputType>() { CustomFieldInputType.Text, CustomFieldInputType.Options };
                    break;

                case CustomFieldDataType.Date:
                    list = new List<CustomFieldInputType>() { CustomFieldInputType.DatePicker, CustomFieldInputType.Options };
                    break;

                case CustomFieldDataType.DateTime:
                    list = new List<CustomFieldInputType>() { CustomFieldInputType.DatePicker };
                    break;

                case CustomFieldDataType.Boolean:
                    list = new List<CustomFieldInputType>() { CustomFieldInputType.Checkbox, };
                    break;

                case CustomFieldDataType.File:
                    list = new List<CustomFieldInputType>() { CustomFieldInputType.File };
                    break;

                default:
                    break;
            }

            return list;
        }

        public OperationResult ValidateModel()
        {
            return OperationResult.Success;
        }

        public static bool IsValidOption(string option, CustomFieldDataType dataType)
        {
            switch (dataType)
            {
                case CustomFieldDataType.RawText:
                    return true;

                case CustomFieldDataType.Number:
                    return int.TryParse(option, out _);

                case CustomFieldDataType.Date:
                case CustomFieldDataType.DateTime:
                    return DateTime.TryParse(option, out _);

                case CustomFieldDataType.Boolean:
                    return bool.TryParse(option, out _);

                default:
                    return true;
            }
        }
    }

    public class CustomizeQuoteRequestVM
    {
        public string Lang { get; set; }

        public CustomizeFieldVM CityAndState { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Address { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Phone { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Date { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM Preference { get; set; } = new CustomizeFieldVM();
        public CustomizeFieldVM QuoteMax { get; set; } = new CustomizeFieldVM();

        public CustomizeQuoteRequestVM()
        {
        }

        public CustomizeQuoteRequestVM(string lang, MaaSConfig config)
        {
            var quote = config.Quote;

            Address = new CustomizeFieldVM(lang, "", "ADDRESS_INPUT", quote.Address.Enabled, quote.Address.Mandatory, quote.Address.IsPublic);
            CityAndState = new CustomizeFieldVM(lang, "", "CITY_AND_STATE_LABEL", quote.CityAndState.Enabled, quote.CityAndState.Mandatory, quote.CityAndState.IsPublic);
            Date = new CustomizeFieldVM(lang, "", "QUOTE_DATE_LABEL", quote.Date.Enabled, quote.Date.Mandatory, quote.Date.IsPublic);
            Phone = new CustomizeFieldVM(lang, "", "PHONE_LABEL", quote.Phone.Enabled, quote.Phone.Mandatory, quote.Phone.IsPublic);
            Preference = new CustomizeFieldVM(lang, "", "QUOTE_PREFERENCE_LABEL", quote.Preference.Enabled, quote.Preference.Mandatory, quote.Preference.IsPublic);
            QuoteMax = new CustomizeFieldVM(lang, "", "QUOTE_MAX_BUDGET_LABEL", quote.QuoteMax.Enabled, quote.QuoteMax.Mandatory, quote.QuoteMax.IsPublic);
        }

        public void CopyTo(MaaSConfig config)
        {
            var quote = config.Quote;

            quote.Address.Enabled = Address.IsEnabled;
            quote.Address.Mandatory = Address.IsMandatory;
            quote.Address.IsPublic = Address.IsPublic;

            quote.CityAndState.Enabled = CityAndState.IsEnabled;
            quote.CityAndState.Mandatory = CityAndState.IsMandatory;
            quote.CityAndState.IsPublic = CityAndState.IsPublic;

            quote.Date.Enabled = Date.IsEnabled;
            quote.Date.Mandatory = Date.IsMandatory;
            quote.Date.IsPublic = Date.IsPublic;

            quote.Phone.Enabled = Phone.IsEnabled;
            quote.Phone.Mandatory = Phone.IsMandatory;
            quote.Phone.IsPublic = Phone.IsPublic;

            quote.QuoteMax.Enabled = QuoteMax.IsEnabled;
            quote.QuoteMax.Mandatory = QuoteMax.IsMandatory;
            quote.QuoteMax.IsPublic = QuoteMax.IsPublic;

            quote.Preference.Enabled = Preference.IsEnabled;
            quote.Preference.Mandatory = Preference.IsMandatory;
            quote.Preference.IsPublic = Preference.IsPublic;
        }
    }

    public class CustomizeUserConfigVM
    {
        public UserConfiguration UserConfig { get; set; } = new UserConfiguration();
        public List<CustomLinksConfigVM> CustomLinks { get; set; } = new List<CustomLinksConfigVM>(3);
    }

    public class CustomLinksConfigVM
    {
        [RequiredIfLocalized("Enabled")]
        [StringLength(200)]
        public string Url { get; set; }

        [RequiredIfLocalized("Enabled")]
        [StringLength(40)]
        public string Label { get; set; }

        public bool Enabled { get; set; }

        public CustomLinksConfigVM()
        {
        }

        public CustomLinksConfigVM(AdditionalLinksConfig linksConfig)
        {
            Url = linksConfig.Url;
            Label = linksConfig.Label;
            Enabled = linksConfig.Enabled;
        }
    }
}