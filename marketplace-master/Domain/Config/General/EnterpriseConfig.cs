using Domain.Config.Common;
using Framework;

namespace Application.Config.General
{
    public class EnterpriseConfig : IConfigNode
    {
        public string DocumentNumber { get; set; } = "123.123.123/0001-12";
        public string Name { get; set; } = "Razão Social da Empresa Ltda.";
        public string CustomerServiceEmail { get; set; } = "sac@nomedastartup.com.br";
        public string TermsOfUseUrl { get; set; }
        public string PrivacyPolicyUrl { get; set; }
        public SocialNetworksConfig SocialNetworks { get; set; } = new SocialNetworksConfig();

        public ErrorDictionary CheckErrors()
        {
            var errors = new ErrorDictionary();
            errors.Merge(this.CheckChildErrors());
            return errors;
        }

        public ErrorDictionary CheckWarnings()
        {
            var warnings = new ErrorDictionary();

            if (DocumentNumber == null)
                warnings.Add(nameof(DocumentNumber), "It is the company's legal ID and is required in attendance to the ecommerce laws.");

            if (Name == null)
                warnings.Add(nameof(Name), "It is the company's Name and is used in several pages and email templates.");

            if (CustomerServiceEmail == null)
                warnings.Add(nameof(CustomerServiceEmail), "It is required in attendance to the ecommerce laws.");

            if (TermsOfUseUrl == null)
                warnings.Add(nameof(TermsOfUseUrl), "It is required in attendance to the ecommerce laws.");

            if (PrivacyPolicyUrl == null)
                warnings.Add(nameof(PrivacyPolicyUrl), "It is required in attendance to the ecommerce laws.");

            warnings.Merge(this.CheckChildWarnings());
            return warnings;
        }

        public void Sanitize()
        {
            ConfigNodeHelper.SanitizeSelfAndChildren(this);
        }
    }
}