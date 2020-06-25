using Domain.Modules.Portfolio;

namespace Domain.Events.PortfolioModule
{
    public class PortfolioUpdatedEvent : DomainEvent
    {
        public Portfolio Portfolio { get; }

        public PortfolioUpdatedEvent(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }
    }
}