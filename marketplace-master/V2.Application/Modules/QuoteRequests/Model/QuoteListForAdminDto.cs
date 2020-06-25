using Domain.QuoteRequests;
using System;

namespace V2.Application.Modules.QuoteRequests.Model
{
    public class QuoteListForAdminDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string PortfolioItemTitle { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public QuoteDealResult DealResult { get; set; }
        public int AnswersCount { get; set; }
        public DateTime? LatestInteraction { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public string SellerEmail { get; set; }
        public string CityAndState { get; set; }
    }
}
