using Application.Config.Common;

namespace Domain.Config.Modules
{
    public class BatchImportConfig
    {
        public ProductsConfig Products { get; set; } = new ProductsConfig();
        public SellersConfig Sellers { get; set; } = new SellersConfig();

        public class ProductsConfig : StatusConfig
        {
            public ProductsConfig() : base(true) { }
        }

        public class SellersConfig : StatusConfig
        {
            public SellersConfig() : base(true) { }
        }
    }
}
