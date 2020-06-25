using Application.Config.Common;

namespace Application.Config.Registers
{
    public class SellerAddressConfig : RegisterConfig
    {
        public bool ShowText { get; set; }

        public bool ShowMap { get; set; }

        public SellerAddressConfig(bool showText = true, bool showMap = true, bool enabled = true, bool mandatory = true)
            : base(enabled, mandatory)
        {
            ShowText = showText;
            ShowMap = showMap;
        }
    }
}
