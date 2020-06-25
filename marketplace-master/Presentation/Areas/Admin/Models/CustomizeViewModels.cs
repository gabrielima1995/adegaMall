using Domain.DTO;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Presentation.Areas.Admin.Models
{
    public enum CustomizeOption
    {
        FavIcon,
        HomeBackground,
        HomeBenefitsIcon1,
        HomeBenefitsIcon2,
        HomeBenefitsIcon3,
        LogoHome,
        LogoInternal,
        PrivacyPolicy,
        SellerBenefitsIcon1,
        SellerBenefitsIcon2,
        SellerBenefitsIcon3,
        Background2,
        UseTerms
    }

    public class CustomizeFileInfoVM
    {
        [Required]
        public CloudFileType FileType { get; set; }

        [Required]
        public string RedirectUrl { get; set; }

        [Required]
        public CustomizeOption Option { get; set; }

        public bool UiNoBox { get; set; }

        public string CurrentUrl { get; set; }

        public string Label { get; set; }

        public string HelpText { get; set; }

        public string Hint { get; set; }

        public string TemplateUrl { get; set; }

        public bool CurrentUrlIsImage => MyUrlHelper.IsImage(CurrentUrl);
    }

    public class CustomizeHtmlVM
    {
        public string HtmlHead { get; set; }

        public string HtmlBodyStart { get; set; }

        public string HtmlBodyEnd { get; set; }

        [DisplayNameLocalized("CUSTOMIZE_HTML_AGREE_TO_TERMS_LABEL")]
        [RequiredLocalized("CUSTOMIZE_HTML_AGREE_TO_TERMS_REQUIRED")]
        public bool AgreeToTerms { get; set; }
    }

    internal static class MyUrlHelper
    {
        public static bool IsImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            try
            {
                var imgExtensions = ".bmp.gif.jpg.jpeg.png.svg";
                var isAbsolute = url.StartsWith("http", StringComparison.OrdinalIgnoreCase);
                var internalUrl = isAbsolute ? url : "http://www.ideianoar.com.br" + (url.StartsWith("/") ? url : "/" + url);
                var uri = new Uri(internalUrl, UriKind.Absolute);
                var file = uri.GetLeftPart(UriPartial.Path);
                var isImage = imgExtensions.Contains(Path.GetExtension(file), StringComparison.OrdinalIgnoreCase);
                return isImage;
            }
            catch { return false; }
        }
    }
}