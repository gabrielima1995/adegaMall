using Domain.DTO;
using Domain.Entities;

namespace Presentation.Models
{
    public class QuoteRequestVM
    {
        public QuoteVM QuoteData { get; set; }

        public string BuyerEmail { get; set; }

        public string BuyerName { get; set; }

        public string SellerId { get; set; }

        public Seller Seller { get; set; }

        public bool HasBuyer => BuyerEmail != null;

        public QuoteRequestVM()
        {
        }

        public QuoteRequestVM(QuoteRequest quoteRequest, bool isLoggedInAsSeller, bool isLoggedInAsBuyer, Seller seller = null)
        {
            BuyerEmail = quoteRequest.BuyerEmail;
            BuyerName = quoteRequest.BuyerName;
            Seller = seller;
            SellerId = quoteRequest.SellerId;
            QuoteData = new QuoteVM(quoteRequest.QuoteData, null, isLoggedInAsSeller, isLoggedInAsBuyer);

            if (QuoteData.BuyerEmail == null) QuoteData.BuyerEmail = BuyerEmail;
            if (QuoteData.BuyerName == null) QuoteData.BuyerName = BuyerName;
            if (QuoteData.SellerId == null) QuoteData.SellerId = SellerId;
            if (QuoteData.SellerAvatar == null) QuoteData.SellerAvatar = Seller?.Avatar;
            if (QuoteData.SellerName == null) QuoteData.SellerName = Seller?.PublicProfileName;
        }
    }
}