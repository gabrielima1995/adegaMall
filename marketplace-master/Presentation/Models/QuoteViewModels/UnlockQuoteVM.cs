using System;

namespace Presentation.Models.QuoteViewModels
{
    public class UnlockQuoteVM
    {
        public int CurrentCredits { get; set; }

        public int UnlockCost { get; set; }

        public Guid QuoteId { get; set; }
    }
}