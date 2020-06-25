using Domain.Entities.PortfolioModule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class LeadSalesProfile : BaseEntity<Guid>
    {
        public LeadSalesProfile()
        {
        }

        public LeadSalesProfile(Guid sellerId)
        {
            Id = Guid.NewGuid();
            SellerId = sellerId;
        }

        public Guid SellerId { get; private set; }
        public List<UnlockedQuote> QuoteRequestsUnlocked { get; private set; } = new List<UnlockedQuote>();
        public List<CreditsBundle> CreditsBundles { get; private set; } = new List<CreditsBundle>();
        public List<CreditsPackTransaction> CreditsPackTransactions { get; private set; } = new List<CreditsPackTransaction>();
        public int TotalAvailableCredits => CreditsBundles.Sum(x => x.AvailableCredits);

        public void UnlockQuote(Quote quote)
        {
            if (!QuoteRequestsUnlocked.Any(x => x.UnlockedQuoteId == quote.Id))
                QuoteRequestsUnlocked.Add(new UnlockedQuote(quote.Id, Id));
        }

        public CreditsBundle AddCredits(int amount, DateTime expirationDate)
        {
            var bundle = new CreditsBundle(amount, expirationDate);
            CreditsBundles.Add(bundle);
            return bundle;
        }

        public void RemoveCredits(int removeAmount)
        {
            var orderedBundles = CreditsBundles.Where(x => x.ExpirationDate > DateTime.UtcNow).OrderBy(x => x.ExpirationDate).ToList();

            while (removeAmount > 0 && TotalAvailableCredits > 0)
            {
                var bundle = orderedBundles.First(x => x.CurrentCredits > 0);
                var amountToRemove = Math.Min(bundle.CurrentCredits, removeAmount);
                bundle.RemoveCredits(amountToRemove);
                removeAmount -= amountToRemove;
            }
        }

        public bool IsQuoteUnlocked(Guid quoteId)
        {
            return QuoteRequestsUnlocked.Any(x => x.UnlockedQuoteId == quoteId);
        }
    }

    public class UnlockedQuote : BaseEntity<Guid>
    {
        public UnlockedQuote()
        {
            Id = Guid.NewGuid();
        }

        public UnlockedQuote(Guid quoteId, Guid salesProfileId)
        {
            UnlockedQuoteId = quoteId;
            LeadSalesProfileId = salesProfileId;
            Id = Guid.NewGuid();
        }

        public Guid UnlockedQuoteId { get; private set; }
        public Guid LeadSalesProfileId { get; private set; }
    }
}
