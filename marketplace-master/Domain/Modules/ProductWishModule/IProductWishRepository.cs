using Domain.Entities.StoreModule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Modules.ProductWishModule
{
    public interface IProductWishRepository
    {
        ProductWish Get(string buyerId, Guid productId);

        void Add(ProductWish productWish);

        void Delete(ProductWish productWish);

        IList<Guid> VerifyProductsWish(string buyerId, IEnumerable<Guid> productIds);

        IQueryable<Product> GetWishlist(string buyerId);
    }
}