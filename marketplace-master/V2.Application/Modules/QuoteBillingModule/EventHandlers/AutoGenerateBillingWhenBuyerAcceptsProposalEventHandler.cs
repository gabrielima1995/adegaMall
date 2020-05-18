using Application.Config;
using Domain.Events;
using Domain.Events.QuoteRequests;

namespace V2.Application.Modules.QuoteBillingModule.EventHandlers
{
    public class AutoGenerateBillingWhenBuyerAcceptsProposalEventHandler : IDomainEventHandler<QuoteClosedByBuyerEvent>
    {
        private readonly QuoteBillingAppService quoteBillingAppService;

        public AutoGenerateBillingWhenBuyerAcceptsProposalEventHandler(QuoteBillingAppService quoteBillingAppService)
        {
            this.quoteBillingAppService = quoteBillingAppService;
        }

        public void Handle(QuoteClosedByBuyerEvent domainEvent)
        {
            if (!IsFeatureTurnedOn())
                return;

            quoteBillingAppService.CreateBillingFromQuote(domainEvent.QuoteId);
        }

        private bool IsFeatureTurnedOn()
        {
            var config = MaaSConfig.Current;

            return
                config.Quote.Enabled &&
                config.Quote.BuyerDecision.Enabled &&
                config.Quote.QuoteBilling.Enabled &&
                config.Quote.QuoteBilling.AutoGenerateBillingWhenBuyerAcceptsProposal;
        }
    }
}