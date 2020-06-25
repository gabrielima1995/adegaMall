using Domain.Core;
using FluentValidation;
using Framework;
using System;

namespace Domain.QuoteRequests.Commands
{
    public class AcceptQuoteCommand : Command
    {
        public Guid QuoteId { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime Deadline { get; private set; }

        public AcceptQuoteCommand(Guid quoteId, decimal amount, DateTime deadline) : base(quoteId.ToString())
        {
            Guard.Ensure.NotEmpty(quoteId, nameof(quoteId));
            QuoteId = quoteId;
            Amount = amount;
            Deadline = deadline;
        }
    }
}
