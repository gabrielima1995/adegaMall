using Domain.Entities.StoreModule;
using Domain.Modules.ProductWishModule;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace V2.Infrastructure.Modules.ProductWishModule
{
    public class ProductWishRepository : IProductWishRepository
    {
        private readonly ScaffoldContext _context;

        public ProductWishRepository(ScaffoldContext context)
        {
            _context = context;
        }

        public ProductWish Get(string buyerId, Guid productId)
        {
            return _context.ProductWishs.FirstOrDefault(i => i.Buyer_Id == buyerId && i.Product_Id == productId);
        }

        public void Add(ProductWish productWish)
        {
            _context.ProductWishs.Add(productWish);
        }

        public void Delete(ProductWish productWish)
        {
            _context.ProductWishs.Remove(productWish);
        }

        public IList<Guid> VerifyProductsWish(string buyerId, IEnumerable<Guid> productIds)
        {
            return _context.ProductWishs
                .Where(x => x.Buyer_Id == buyerId)
                .Where(x => productIds.Contains(x.Product_Id))
                .Select(x => x.Product_Id)
                .ToList();

        }

        public IQueryable<Product> GetWishlist(string buyerId)
        {

            return _context.ProductWishs
                .Include(pw => pw.Product)
                .Where(pw => pw.Buyer_Id == buyerId)
                .Select(pw => pw.Product);

        }
    }
}