using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class WebhookRepository : EfCrudRepository<Webhook>, IWebhookRepository
    {
        public WebhookRepository(ScaffoldContext context) : base(context)
        {
        }

        private IQueryable<WebhookListing> QueryForPage(string sellerId = null)
        {
            return Context.Set<Webhook>()
                .Where(x => !x.Deleted)
                .Where(x => x.ApiToken.Seller.Id == sellerId)
                .Select(x => new WebhookListing
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    EventType = x.EventType,
                    Url = x.Url
                });
        }

        public Tuple<IList<WebhookListing>, int> ListForPage(int start, int limit, string sellerId = null)
        {
            var query = QueryForPage(sellerId);

            var data = query
                .OrderByDescending(i => i.CreatedAt)
                .Skip(start)
                .Take(limit)
                .ToList();

            var total = query
                .Count();

            return new Tuple<IList<WebhookListing>, int>(data, total);
        }

        public List<WebhookToNotify> ListToNotify(WebhookEvent eventType, string sellerId)
        {
            return Context.Set<Webhook>()
                .Include(x => x.ApiToken)
                .Where(x => !x.Deleted)
                .Where(x => x.EventType == eventType)
                .Where(x => x.ApiToken.Seller.Id == null || x.ApiToken.Seller.Id == sellerId) // obtem os webhooks gerais e os webhooks do seller do recurso
                .Select(x => new WebhookToNotify
                {
                    Id = x.Id,
                    EventType = x.EventType,
                    Url = x.Url,
                    ApiToken = x.ApiToken.Token,
                    SellerId = x.ApiToken.Seller_Id
                })
                .ToList();
        }
    }
}