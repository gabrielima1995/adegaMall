using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Products
{
    public class CategoryMenuAppService
    {
        private const string VINHOS = "vinhos";
        private const string DESTILADOS = "destilados";
        private const string CERVEJAS = "cervejas";
        private const string GOURMET = "gourmet";

        private const string POR_PRECO = "preco";
        private const string POR_PAIS = "pais";
        private const string POR_TIPO_VINHO = "vinho";

        public static readonly string[] CategoriesAllow =
        {
            VINHOS, DESTILADOS, CERVEJAS, GOURMET
        };

        private static readonly string[] VariantTypesAllow =
        {
            POR_PRECO, POR_PAIS, POR_TIPO_VINHO
        };

        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductVariantType> _productVariantTypeRepository;

        public CategoryMenuAppService(
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductVariantType> productVariantTypeRepository)
        {
            _productCategoryRepository = productCategoryRepository;
            _productVariantTypeRepository = productVariantTypeRepository;
        }

        public List<CategoryMenu> Get()
        {
            var categories = _productCategoryRepository
                .Query()
                .Where(i => CategoriesAllow.Contains(i.Slug))
                .Where(i => !i.Deleted)
                .ToList();

            var variants = _productVariantTypeRepository
                .Query(i => i.Variants)
                .Where(i => VariantTypesAllow.Contains(i.Slug))
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Order)
                .ThenBy(i => i.Name)
                .ToList();

            var menus = new List<CategoryMenu>(categories.Count);
            var items = new List<CategoryMenuItem>(3);

            #region Variantes Por preço

            var porPreco = variants.FirstOrDefault(i => i.Slug == POR_PRECO);

            if (porPreco != null && porPreco.Variants.Any(i => !i.Deleted))
            {
                items.Add(new CategoryMenuItem(porPreco,
                    $"Por {porPreco.Name.ToLower()}",
                    porPreco.Variants
                        .Where(i => !i.Deleted)
                        .OrderBy(i => i.Order)
                        .ThenBy(i => i.Name)
                        .ToList()));
            }

            #endregion Variantes Por preço

            #region Variantes Por país

            var porPais = variants.FirstOrDefault(i => i.Slug == POR_PAIS);

            if (porPais != null && porPais.Variants.Any(i => !i.Deleted))
            {
                items.Add(new CategoryMenuItem(porPais,
                    $"Por {porPais.Name.ToLower()}",
                    porPais.Variants
                        .Where(i => !i.Deleted)
                        .OrderBy(i => i.Order)
                        .ThenBy(i => i.Name)
                        .ToList()));
            }

            #endregion Variantes Por país

            #region Categoria Vinhos

            var vinhos = categories.FirstOrDefault(i => i.Slug == VINHOS);

            if (vinhos != null)
            {
                var vinhosItems = items.ToList();

                #region Variantes Por tipo de vinho

                var porTipoVinho = variants.FirstOrDefault(i => i.Slug == POR_TIPO_VINHO);

                if (porTipoVinho != null && porTipoVinho.Variants.Any(i => !i.Deleted))
                {
                    vinhosItems.Add(new CategoryMenuItem(porTipoVinho,
                        $"Por {porTipoVinho.Name.ToLower()}",
                        porTipoVinho.Variants
                            .Where(i => !i.Deleted)
                            .OrderBy(i => i.Order)
                            .ThenBy(i => i.Name)
                            .ToList()));
                }

                #endregion Variantes Por tipo de vinho

                menus.Add(new CategoryMenu(vinhos,
                    "Ver todos", $"Ver todos os {vinhos.Name.ToLower()}",
                    vinhosItems));
            }

            #endregion Categoria Vinhos

            #region Categoria Destilados

            var destilados = categories.FirstOrDefault(i => i.Slug == DESTILADOS);

            if (destilados != null) menus.Add(new CategoryMenu(destilados,
                "Ver todos", $"Ver todos os {destilados.Name.ToLower()}",
                items));

            #endregion Categoria Destilados

            #region Categoria Cervejas

            var cervejas = categories.FirstOrDefault(i => i.Slug == CERVEJAS);

            if (cervejas != null) menus.Add(new CategoryMenu(cervejas,
                "Ver todas", $"Ver todas as {cervejas.Name.ToLower()}",
                items));

            #endregion Categoria Cervejas

            #region Categoria Licorosos

            var licorosos = categories.FirstOrDefault(i => i.Slug == GOURMET);

            if (licorosos != null) menus.Add(new CategoryMenu(licorosos,
                "Ver todos", $"Ver todos os {licorosos.Name.ToLower()}",
                items));

            #endregion Categoria Licorosos

            return menus;
        }
    }
}