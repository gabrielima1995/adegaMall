using Domain.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IWebhookRepository : ICrudRepository<Webhook>
    {
        Tuple<IList<WebhookListing>, int> ListForPage(int start, int limit, string sellerId = null);

        List<WebhookToNotify> ListToNotify(WebhookEvent eventType, string sellerId);
    }
}