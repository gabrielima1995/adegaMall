using Application.Config.Common;
using Application.Config.Modules.Portfolio;

namespace Application.Config.Modules
{
    public class PortfolioConfig : StatusConfig
    {
        public int ItemsLimit { get; set; }

        public int CategoriesLimit { get; set; } = 10;

        public bool ShowRelatedItems { get; set; } = false;

        public bool ShowOwnPage { get; set; } = false;

        public StatusConfig VideoUrl { get; set; } = new StatusConfig();

        public RegisterConfig Price { get; set; } = new RegisterConfig();

        public PortfolioSearchConfig Search { get; set; } = new PortfolioSearchConfig();

        public StatusConfig ThumbnailCrop { get; set; } = new StatusConfig();

        public RegisterConfig PortfolioItems { get; set; } = new RegisterConfig(true, true);

        public PortfolioConfig()
            : base(enabled: false)
        {
        }
    }
}