using Domain.Entities.PortfolioModule;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.QuoteRequests.Queries
{
    public class QuoteGroupDetailsForBuyer
    {
        public bool AllClosed { get; set; }
        public Quote Quote { get; set; }
        public int TotalSellersCount { get; set; }
        public IEnumerable<QuoteGroupInteractionForBuyer> SellerInteractions { get; set; }
    }

    public class QuoteGroupInteractionForBuyer
    {
        public string SellerId { get; set; }
        public Guid QuoteId { get; set; }
        public string SellerName { get; set; }
        public string SellerNickname { get; set; }
        public string SellerAvatarUrl { get; set; }
        public bool HasInteraction { get; set; }
        public string InteractionText { get; set; }
        public decimal? InteractionPrice { get; set; }
        public DateTime? InteractionDeadline { get; set; }
        public DateTime? InteractionDate { get; set; }
        public string AttachmentUrl { get; set; }
        public bool IsInteractionBySeller { get; set; }
    }
}
