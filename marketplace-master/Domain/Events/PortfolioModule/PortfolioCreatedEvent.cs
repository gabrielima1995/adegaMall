using Domain.Modules.Portfolio;

namespace Domain.Events.PortfolioModule
{
    public class PortfolioCreatedEvent : DomainEvent
    {
        public Portfolio Portfolio { get; }

        public PortfolioCreatedEvent(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }
    }
}