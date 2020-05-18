using Infra.SmartSearchAdapter.Common;

namespace Infra.SmartSearchAdapter.Products
{
    public class ProductIndexContext : BaseIndexContext
    {
        public ProductIndexContext() : base("ProductsV20200401")
        {
        }
    }
}