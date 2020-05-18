using Application.Config.Common;

namespace Application.Config.Modules
{
    public class PortfolioItemQuoteConfig : StatusConfig
    {
        public RegisterConfig Quantity { get; set; } = new RegisterConfig();

        public PortfolioItemQuoteConfig()
            : base(false)
        {

        }
    }
}