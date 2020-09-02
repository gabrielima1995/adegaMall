using System;
using Domain.Interfaces.Repositories;

namespace Domain.Modules.StoreModule.Coupons
{
    public interface ICouponRepository : ICrudRepository<Coupon>
    {
        Coupon GetByCode(string code);
        bool IsFirstOrder(string buyerId);
        bool IsOneTimeByUser(string buyerId, Guid couponId);
        bool IsOneTimeEver(Guid couponId);
    }
}