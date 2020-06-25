using Domain.Modules.Portfolio;

namespace Domain.Events.PortfolioModule
{
    public class PortfolioItemUpdatedEvent : DomainEvent
    {
        public PortfolioItem PortfolioItem { get; }

        public string SellerId { get; }

        public PortfolioItemUpdatedEvent(string sellerId, PortfolioItem portfolioItem)
        {
            SellerId = sellerId;
            PortfolioItem = portfolioItem;
        }
    }
}