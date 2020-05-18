using Domain.Entities;
using Framework;
using System;

namespace Domain.Sellers
{
    [Obsolete]
    public class SellerService_Obsolete : ServiceBase<Seller>
    {
        public SellerService_Obsolete(IRepository<Seller> repository)
            : base(repository)
        {
        }
    }
}