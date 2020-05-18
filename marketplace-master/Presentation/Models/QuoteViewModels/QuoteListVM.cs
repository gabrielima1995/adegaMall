using Application.Config;
using Domain.Entities.PortfolioModule;
using Domain.Modules.QuoteBillingModule.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Models
{
    public class QuoteListVM
    {
        public List<QuoteVM> Quotes { get; set; } = new List<QuoteVM>();

        public int TotalOpenQuotes { get; set; }

        public int TotalOngoingQuotes { get; set; }

        public int TotalClosedQuotes { get; set; }

        public bool ViewingOpenQuotes => !ViewingClosedQuotes && !ViewingOngoingQuotes;

        public bool ViewingClosedQuotes { get; set; }

        public bool ViewingOngoingQuotes { get; set; }

        public QuoteListVM()
        {
        }

        public QuoteListVM(
            List<Quote> quotes, List<QuoteBilling> quoteBillings,
            int totalOpenQuotes, int totalOngoingQuotes, int totalClosedQuotes,
            bool viewingOngoingQuotes, bool viewingClosedQuotes, List<Guid> unlockedQuotes,
            bool isLoggedInAsSeller, bool isLoggedInAsBuyer)
        {
            var sortedList = quotes.OrderByDescending(
                quote => viewingClosedQuotes ? quote.ClosedAt : quote.CreatedAt).ToList();

            Quotes = sortedList.Select(quote =>
            {
                var billing = quoteBillings.SingleOrDefault(b => b.QuoteId == quote.Id);
                var newQuote = new QuoteVM(quote, billing, isLoggedInAsSeller, isLoggedInAsBuyer);

                if (MaaSConfig.Current.Quote.LeadSales.Enabled)
                {
                    var quoteIsUnlocked = unlockedQuotes.Any(unlockedQuote => unlockedQuote == quote.Id);
                    if (!quoteIsUnlocked)
                        newQuote.QuoteDetailsHidden = true;
                }

                return newQuote;

            }).ToList();

            TotalOpenQuotes = totalOpenQuotes;
            TotalOngoingQuotes = totalOngoingQuotes;
            TotalClosedQuotes = totalClosedQuotes;

            ViewingOngoingQuotes = viewingOngoingQuotes;
            ViewingClosedQuotes = viewingClosedQuotes;
        }
    }
}