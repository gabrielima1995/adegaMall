using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Modules.QuoteBillingModule.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2.Application.Modules.QuoteRequests.Queries
{
    public class QuoteListForBuyer
    {
        public IEnumerable<QuoteListItemForBuyer> Items { get; set; }
        public int TotalQuotes => Items.Count();
    }

    public class QuoteListItemForBuyer : IWithCustomFields
    {
        public Guid Id { get { return Quote.Id; } set { } }

        public Quote Quote { get; set; }
        public QuoteBilling Billing { get; set; }
        public bool AllClosed { get; set; }
        public int TotalSellersCount { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public QuoteListItemForBuyerInteraction LatestInteraction { get; set; }
        public IEnumerable<QuoteListItemForBuyerTopSeller> TopSellers { get; set; }

        public bool IsGroup => Quote.GroupId.HasValue;
        public CustomFieldEntityType EntityType => CustomFieldEntityType.Quote;
        public bool HasAnySellers => TotalSellersCount > 0;
    }

    public class QuoteListItemForBuyerInteraction
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsSeller { get; set; }
        public string Text { get; set; }
        public DateTime OccurredAt { get; set; }
        public decimal? Price { get; set; }
        public DateTime? DeliveryDeadline { get; set; }
        public object SellerNickname { get; set; }
    }

    public class QuoteListItemForBuyerTopSeller
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string AvatarUrl { get; set; }
    }
}
