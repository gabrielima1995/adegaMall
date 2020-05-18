namespace Application.Config.Modules.Portfolio
{
    public class PortfolioSearchConfig
    {
        public bool RequireLogin { get; set; } = false;

        public int ResultsByPage { get; set; } = 20;

        public bool MapSearchEnabled { get; set; } = false;

        public long MapSearchRange { get; set; } = 30000;

        /*
        public StatusConfig ByCategories { get; set; } = new StatusConfig();
        public StatusConfig ByAdditionalInfo2 { get; set; } = new StatusConfig();
        public StatusConfig ByCityAndState { get; set; } = new StatusConfig();
        public StatusConfig ByOnlyAvatar { get; set; } = new StatusConfig();
        */
    }
}