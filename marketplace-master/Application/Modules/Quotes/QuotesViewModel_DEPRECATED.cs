using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Application.Modules.Quotes
{
    public class QuotesViewModel_DEPRECATED
    {
        public IList<Quote> Quotes { get; set; }

        public int TotalOpenedQuotes { get; set; }

        public QuoteStatus QuoteStatus { get; set; }
    }
}