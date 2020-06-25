using Domain.Core;
using FluentValidation;
using Framework;
using System;

namespace Domain.QuoteRequests.Commands
{
    public class CloseQuoteCommand : Command
    {
        public Guid QuoteId { get; private set; }

        public bool DealWon { get; private set; }

        public CloseQuoteCommand(Guid quoteId, bool dealWon) : base(quoteId.ToString())
        {
            Guard.Ensure.NotEmpty(quoteId, nameof(quoteId));

            QuoteId = quoteId;
            DealWon = dealWon;
        }
    }
}
