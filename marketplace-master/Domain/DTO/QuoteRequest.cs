using Domain.Entities.PortfolioModule;

namespace Domain.DTO
{
    public class QuoteRequest
    {
        public Quote QuoteData { get; set; }

        public string BuyerEmail { get; set; }

        public string BuyerName { get; set; }

        public string SellerId { get; set; }

        public QuoteRequest()
        {
        }

        public QuoteRequest(Quote quoteData, string buyerEmail, string buyerName, string sellerId = null)
        {
            QuoteData = quoteData;
            BuyerEmail = buyerEmail;
            BuyerName = buyerName;
            SellerId = sellerId;
        }
    }
}
