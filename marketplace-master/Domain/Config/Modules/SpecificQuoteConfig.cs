using Application.Config.Common;

namespace Application.Config.Modules
{
    public class SpecificQuoteConfig : StatusConfig
    {
        public StatusConfig ShortQuote { get; set; } = new StatusConfig(false);
    }
}