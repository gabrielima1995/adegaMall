using System;

namespace V2.Application.Modules.QuoteRequests.Model
{
    public class QuoteWithoutSellerListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string PortfolioItemTitle { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string CityAndState { get; set; }
        public bool HasBeenSentToAnySellers { get; set; }
    }
}
