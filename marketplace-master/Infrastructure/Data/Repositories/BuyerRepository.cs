using Domain.Entities;
using Domain.Events;
using Domain.Interfaces.Repositories;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using System;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class BuyerRepository : EfCrudRepository<Buyer>, IBuyerRepository,
        IDomainEventHandler<PersonUpdatedEvent>,
        IDomainEventHandler<BuyerUpdatedEvent>
    {
        private const string CacheFmt_GetProfile = "__BuyerRepository_GetProfile_{0}";

        public BuyerRepository(ScaffoldContext context) : base(context)
        {
        }

        public Buyer GetByEmail(string email)
        {
            return CacheHelper.Get(Context.Set<Buyer>().Where(x => x.Email == email));
        }

        public Buyer GetDetailsForAdmin(string buyerId, bool includeOrders, bool includeQuotes)
        {
            var query = Context.Set<Buyer>()
                .Include(x => x.Address)
                .Where(x => x.Id == buyerId);

            if (includeOrders)
            {
                query = query
                    .Include(x => x.Orders)
                    .Include(x => x.Orders.Select(o => o.Products))
                    .Include(x => x.Orders.Select(o => o.Seller))
                    .Include(x => x.Orders.Select(o => o.ShippingAddress))
                    .Include(x => x.ShippingAddresses)
                    .Include(x => x.ShippingAddresses.Select(s => s.Address));
            }

            if (includeQuotes)
            {
                query = query
                    .Include(x => x.Quotes)
                    .Include(x => x.Quotes.Select(q => q.Seller))
                    .Include(x => x.Quotes.Select(q => q.Category))
                    .Include(x => x.Quotes.Select(q => q.SubCategory));
            }

            var result = CacheHelper.Get(query);

            if (result != null)
            {
                if (includeOrders) result.Orders = result.Orders.OrderByDescending(o => o.CreatedAt).ToList();
                if (includeQuotes) result.Quotes = result.Quotes.OrderByDescending(q => q.CreatedAt).ToList();
            }

            return result;
        }

        public Buyer GetProfile(string buyerId)
        {
            var cache = new CacheDefinition(string.Format(CacheFmt_GetProfile, buyerId), new TimeSpan(0, 5, 0));

            return CacheHelper.Get(Context.Set<Buyer>()
                .Where(x => x.Id == buyerId)
                .Include(x => x.Address), cache);
        }

        public Buyer GetWithAddresses(string buyerId)
        {
            return CacheHelper.Get(Context.Set<Buyer>()
                .Where(x => x.Id == buyerId)
                .Include(x => x.Address)
                .Include(x => x.ShippingAddresses)
                .Include(x => x.ShippingAddresses.Select(s => s.Address)));
        }

        #region Events

        private void RemoveCache(string buyerId)
            => CacheHelper.Evict(string.Format(CacheFmt_GetProfile, buyerId));

        public void Handle(PersonUpdatedEvent domainEvent)
            => RemoveCache(domainEvent.PersonId);

        public void Handle(BuyerUpdatedEvent domainEvent)
            => RemoveCache(domainEvent.BuyerId);

        #endregion Events
    }
}