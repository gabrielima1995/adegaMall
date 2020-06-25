using Application.Config.Common;

namespace Application.Config.Registers
{
    public class SellerAssistantConfig : StatusConfig
    {
        public SellerAssistantConfig() : base(false)
        {
        }

        public string MainPageUrl { get; set; } = "/manage";
    }
}