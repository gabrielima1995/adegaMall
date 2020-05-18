using Application.Config;
using Application.Helpers;
using System.Text.RegularExpressions;

namespace V2.Application.Messages
{
    public class MessageModel
    {
        [MessageVariable(TemplateVariable = "[MESSAGE_TITLE]", DescriptionLabel = "INTERNAL_VARIABLE_DESCRIPTION", HideFromUI = true)]
        public string MessageTitle { get; set; }

        [MessageVariable(TemplateVariable = "[APP_DOCUMENT_NUMBER]", DescriptionLabel = "INTERNAL_VARIABLE_DESCRIPTION", HideFromUI = true)]
        public string AppDocumentNumber => MaaSConfig.Current.Enterprise.DocumentNumber;

        [MessageVariable(TemplateVariable = "[TERMS_OF_USE_URL]", DescriptionLabel = "INTERNAL_VARIABLE_DESCRIPTION", HideFromUI = true)]
        public string TermsOfUseUrl => MaaSConfig.Current.Enterprise.TermsOfUseUrl;

        [MessageVariable(TemplateVariable = "[PRIVACY_POLICY_URL]", DescriptionLabel = "INTERNAL_VARIABLE_DESCRIPTION", HideFromUI = true)]
        public string PrivacyPolicyUrl => MaaSConfig.Current.Enterprise.PrivacyPolicyUrl;


        [MessageVariable(TemplateVariable = "[APP_LOGO]", DescriptionLabel = "APP_LOGO_VARIABLE_DESCRIPTION")]
        public string AppLogo => CreateHtmlImgTag(ImgHelper.Get(MaaSConfig.Current.Layout.Images.LogoHome, 160, 160, null,null), 80);

        [MessageVariable(TemplateVariable = "[APP_NAME]", DescriptionLabel = "APP_NAME_VARIABLE_DESCRIPTION")]
        public string AppName => MaaSConfig.Current.ApplicationName;

        [MessageVariable(TemplateVariable = "[APP_URL]", DescriptionLabel = "APP_URL_VARIABLE_DESCRIPTION")]
        public string AppUrl => MaaSConfig.Current.Environment.AppUrl;

        [MessageVariable(TemplateVariable = "[CUSTOMER_SERVICE_EMAIL]", DescriptionLabel = "CUSTOMER_SERVICE_EMAIL_VARIABLE_DESCRIPTION")]
        public string CustomerServiceEmail => MaaSConfig.Current.Enterprise.CustomerServiceEmail;

        [MessageVariable(TemplateVariable = "[FACEBOOK_PAGE_URL]", DescriptionLabel = "FACEBOOK_PAGE_URL_VARIABLE_DESCRIPTION")]
        public string FacebookPageUrl => MaaSConfig.Current.Facebook.PageUrl;

        protected string BuildAppUrl(string relativePath)
        {
            if (AppUrl == null) return relativePath;

            var appUrl = AppUrl.EndsWith("/") ? AppUrl : AppUrl + "/";
            var relative = relativePath.StartsWith("/") ? relativePath.Substring(1) : relativePath;
            return appUrl + relative;
        }

        protected string CreateHtmlImgTag(string imageUrl, int size = 200)
        {
            var notATag = new Regex(@"^(?!<)\S+.(?:gif|png|jpeg|jpg|svg).*$(?<!>)");

            if (notATag.IsMatch(imageUrl))
                return $@"<img height=""{size}"" src=""{imageUrl}"" alt=""Logo"" />";

            return imageUrl;
        }
    }
}
