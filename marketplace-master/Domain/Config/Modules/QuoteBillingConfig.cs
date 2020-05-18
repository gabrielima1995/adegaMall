using Application.Config.Common;

namespace Application.Config.Modules
{
    public class QuoteBillingConfig : StatusConfig
    {
        public bool AutoGenerateBillingWhenBuyerAcceptsProposal { get; set; } = true;
    }
}