using Application.Config.Common;

namespace Application.Config.Modules
{
    public class GeneralQuoteConfig : StatusConfig
    {
        public bool RegisterQuoteEvenWhenNoSellers { get; set; } = true;
        public bool ShowSellersWithoutAnswer { get; set; } = true;
    }
}