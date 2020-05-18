using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Modules.ProductWishModule;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Buyer : Person
    {
        private Buyer() { }
        public Buyer(string userId, string email, string fullName, bool requireModeration) : base(userId, email, requireModeration)
        {
            FullName = fullName;
        }

        public IList<Quote> Quotes { get; set; }
        public IList<Order> Orders { get; set; }
        public IList<ProductWish> ProductWishes { get; set; }

        public static Buyer CreateAsDto() => new Buyer();
        public override PersonType PersonType => PersonType.Buyer;
    }
}