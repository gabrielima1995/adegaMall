using Domain.Modules.Portfolio;

namespace Domain.Events.PortfolioModule
{
    public class PortfolioItemRemovedEvent : DomainEvent
    {
        public PortfolioItem PortfolioItem { get; }
        public string SellerId { get; }

        public PortfolioItemRemovedEvent(string sellerId, PortfolioItem portfolioItem)
        {
            SellerId = sellerId;
            PortfolioItem = portfolioItem;
        }
    }
}