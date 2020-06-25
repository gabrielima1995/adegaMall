using Domain.Core;
using FluentValidation;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.QuoteRequests.Commands
{
    public class AddQuoteAnswerCommand : Command
    {
        public Guid QuoteId { get; private set; }

        public Guid AnswerId { get; private set; }

        public string BuyerId { get; private set; }

        public string SellerId { get; private set; }

        public string AuthorId { get; private set; }

        public string Text { get; private set; }

        public decimal? Price { get; private set; }

        public DateTime? Deadline { get; private set; }

        public List<string> AttachmentUrls { get; private set; } = new List<string>();

        public bool IsModerator { get; private set; }

        public AddQuoteAnswerCommand(Guid quoteId, Guid answerId, string buyerId, string sellerId, string authorId, string text, decimal? price, DateTime? deadline, IEnumerable<string> attachmentUrls, bool isModerator)
            : base(answerId.ToString())
        {
            Guard.Ensure.NotEmpty(quoteId, nameof(quoteId));
            Guard.Ensure.NotEmpty(answerId, nameof(answerId));
            Guard.Ensure.NotEmpty(buyerId, nameof(buyerId));
            Guard.Ensure.NotEmpty(sellerId, nameof(sellerId));
            Guard.Ensure.NotEmpty(authorId, nameof(authorId));
            Guard.Ensure.NotEmpty(text, nameof(text));

            if (!isModerator && authorId != buyerId && authorId != sellerId)
                throw new ArgumentOutOfRangeException($"Parameter {nameof(authorId)} must equal {nameof(buyerId)} or {nameof(sellerId)}.");

            if (price != null)
                Guard.Ensure.GreaterOrEqual(price.Value, 0M, nameof(price));

            QuoteId = quoteId;
            AnswerId = answerId;
            BuyerId = buyerId;
            SellerId = sellerId;
            AuthorId = authorId;
            Text = text;
            Price = price;
            Deadline = deadline;

            if (attachmentUrls != null)
                AttachmentUrls.AddRange(attachmentUrls);

            IsModerator = isModerator;
        }
    }
}