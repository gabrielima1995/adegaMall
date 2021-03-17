using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Coupons;
using Infrastructure.Data.Context;
using System;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class CouponRepository : EfCrudRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(ScaffoldContext context) : base(context)
        {
        }

        public Coupon GetByCode(string code)
        {
            return Context.Set<Coupon>()
                .Include(x => x.Seller)
                .FirstOrDefault(i => i.Code == code);
        }

        public bool IsFirstOrder(string buyerId)
        {
            return Context.Set<Order>()
                .Where(i => i.Status != OrderStatus.Processed)
                .Count(i => i.Buyer.Id == buyerId) == 0;
        }

        public bool IsOneTimeByUser(string buyerId, Guid couponId)
        {
            return Context.Set<Order>()
                .Where(i => i.Status != OrderStatus.Processed && i.Status != OrderStatus.Canceled)
                .Count(i => i.Buyer.Id == buyerId && i.Coupon.Id == couponId) == 0;
        }

        public bool IsOneTimeEver(Guid couponId)
        {
            return Context.Set<Order>()
                .Where(i => i.Status != OrderStatus.Processed)
                .Count(i => i.Coupon.Id == couponId) == 0;
        }
    }
}
