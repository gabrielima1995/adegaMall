using Application.Config.Common;

namespace Domain.Config.Sections
{
    public class EcommerceProductsConfig : StatusConfig
    {
        public int MaxCategories { get; set; } = 300;
        public bool EnableRandomOrder { get; set; } = true;
    }
}
