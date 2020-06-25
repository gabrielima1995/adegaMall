using Domain.Entities.StoreModule;
using Framework;
using System.Linq;

namespace V2.Application.Modules.Search.Products
{
    public static class PaisBadgeExtension
    {
        public static string[] Paises { get; private set; }

        public static void LoadPaises(IRepository<ProductVariant> productVariantRepository)
        {
            Paises = productVariantRepository
                .Query()
                .Where(i => i.Type.Slug == "pais")
                .Select(i => i.Slug)
                .ToArray();
        }

        public static string GetPaisBadge(this ProductSearchResultItem item)
        {
            return item.VariantSlugs?.FirstOrDefault(i => Paises.Contains(i));
        }

        public static string GetPaisBadge(this Product item)
        {
            return item.Variants?.FirstOrDefault(i => Paises.Contains(i.Slug))?.Slug;
        }
    }
}