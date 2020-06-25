using Application.Modules.Products.Queries;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Modules.StoreModule.Price;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Products
{
    public class ProductWithPrice
    {
        public ProductWithPrice(Product product)
        {
            Product = product;
        }

        public Product Product { get; }
        public ProductPrice ProductPrice => Product.ProductPriceMapped;
    }

    public class ProductSearchStore
    {
        private const string ProductSearchKey = "PRODUCT_SEARCH_CACHE";
        private static TimeSpan cacheTimeout;

        private readonly ICache cache;
        private readonly IProductSearchQueries productSearchQueries;

        static ProductSearchStore()
        {
#if DEBUG
            cacheTimeout = new TimeSpan(0, 0, 10);
#else
            cacheTimeout = new TimeSpan(0, 10, 0);
#endif
        }

        public ProductSearchStore(ICache cache, IProductSearchQueries productSearchQueries)
        {
            this.cache = cache;
            this.productSearchQueries = productSearchQueries;
        }

        public IReadOnlyList<ProductWithPrice> GetProducts()
        {
            if (!cache.Contains(ProductSearchKey))
            {
                var products = GetProductsInternal();
                var productsWithPrice = AddPrice(products);
                cache.Add(ProductSearchKey, productsWithPrice, cacheTimeout, CacheTimeoutType.Absolute);
            }

            return cache.Get<IReadOnlyList<ProductWithPrice>>(ProductSearchKey);
        }

        private IReadOnlyList<Product> GetProductsInternal()
        {
            var products = productSearchQueries.ListAll();
            return products.ToList();
        }

        private IReadOnlyList<ProductWithPrice> AddPrice(IEnumerable<Product> products)
        {
            var list = products as IList<Product> ?? products.ToList();

            var productsWithPrice = new List<ProductWithPrice>(list.Count);

            productsWithPrice.AddRange(list.Select(product => new ProductWithPrice(product)));

            return productsWithPrice;
        }
    }
}