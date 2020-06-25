using FluentValidation;
using System;
using System.Linq;

namespace Domain.Events.QuoteRequests
{
    public class QuoteAnswerAddedEvent : DomainEvent
    {
        public Guid QuoteId { get; private set; }

        public Guid AnswerId { get; private set; }

        public string BuyerId { get; private set; }

        public string SellerId { get; private set; }

        public string AuthorId { get; private set; }

        public bool AuthorIsModerator { get; private set; }

        public bool AuthorIsBuyer => AuthorId == BuyerId;
        public bool AuthorIsSeller => AuthorId == SellerId;

        public QuoteAnswerAddedEvent(Guid quoteId, Guid answerId, string sellerId, string buyerId, string authorId, bool isModerator)
        {
            QuoteId = quoteId;
            AnswerId = answerId;
            SellerId = sellerId;
            BuyerId = buyerId;
            AuthorId = authorId;
            AuthorIsModerator = isModerator;

            CheckInvariants();
        }

        private void CheckInvariants()
        {
            var result = new QuoteAnswerAddedEventValidator().Validate(this);
            var errors = string.Join(Environment.NewLine, result.Errors.Select(x => x.ErrorMessage));
            if (!result.IsValid) throw new Exception("Validation failed for QuoteAnswerAddedEvent. Errors: " + errors);
        }
    }

    internal class QuoteAnswerAddedEventValidator : AbstractValidator<QuoteAnswerAddedEvent>
    {
        public QuoteAnswerAddedEventValidator()
        {
            RuleFor(x => x.QuoteId)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.AnswerId)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.BuyerId)
                .NotEmpty();

            RuleFor(x => x.SellerId)
                .NotEmpty();

            RuleFor(x => x.AuthorId)
                .Equal(x => x.BuyerId)
                .Unless(x => x.AuthorId == x.SellerId || x.AuthorIsModerator);

            RuleFor(x => x.AuthorId)
                .Equal(x => x.SellerId)
                .Unless(x => x.AuthorId == x.BuyerId || x.AuthorIsModerator);
        }
    }
}