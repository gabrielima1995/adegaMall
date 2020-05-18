using System;

namespace Presentation.Models
{
    public class AcceptQuoteVM
    {
        public Guid QuoteId { get; set; }

        /// <summary>
        /// The anwswer that contains the price and deadline accepted by the buyer.
        /// </summary>
        public Guid QuoteAnswerId { get; set; }
    }
}