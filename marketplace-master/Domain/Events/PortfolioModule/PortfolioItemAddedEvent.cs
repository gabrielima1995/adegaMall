using Domain.Modules.Portfolio;

namespace Domain.Events.PortfolioModule
{
    public class PortfolioItemAddedEvent : DomainEvent
    {
        public PortfolioItem PortfolioItem { get; }

        public string SellerId { get; }

        public PortfolioItemAddedEvent(string sellerId, PortfolioItem portfolioItem)
        {
            SellerId = sellerId;
            PortfolioItem = portfolioItem;
        }
    }
}