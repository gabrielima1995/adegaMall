using Domain.Core;
using Domain.Entities.PortfolioModule;
using FluentValidation;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.QuoteRequests.Commands
{
    public class RequestQuoteCommand : Command
    {
        public Guid QuoteId { get; private set; }

        public string BuyerId { get; private set; }

        public IEnumerable<string> SellerIds { get; private set; }

        public Guid AddressId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Phone { get; private set; }

        public DateTime? Date { get; private set; }

        public QuotePreference Preference { get; private set; }

        public decimal? Amount { get; private set; }

        public int? CategoryId { get; private set; }

        public int? SubCategoryId { get; private set; }

        public List<string> AttachmentUrls { get; private set; } = new List<string>();

        public bool IsGeneralQuoteRequest { get; private set; }

        public bool IsShortQuote { get; private set; }

        public Guid? PortfolioItemId { get; private set; }

        public int Quantity { get; private set; }

        public RequestQuoteCommand(
            Guid quoteId, string title, string description, string phone, DateTime? date,
            QuotePreference preference, decimal? amount,
            int? categoryId, int? subCategoryId, string buyerId, IEnumerable<string> sellerIds, Guid addressId,
            IEnumerable<string> attachmentUrls, bool isGeneralQuoteRequest, bool isShortQuote,
            Guid? portfolioItemId, int quantity)
                : base(quoteId.ToString())
        {
            Guard.Ensure.NotEmpty(quoteId, nameof(quoteId));
            Guard.Ensure.NotEmpty(quoteId, nameof(buyerId));
            Guard.Ensure.NotEmpty(quoteId, nameof(sellerIds));
            Guard.Ensure.NotEmpty(quoteId, nameof(description));

            QuoteId = quoteId;
            Title = title;
            Description = description;
            Phone = phone;
            Date = date;
            Preference = preference;
            Amount = amount;
            CategoryId = categoryId;
            SubCategoryId = subCategoryId;
            BuyerId = buyerId;
            SellerIds = sellerIds ?? new string[0];
            AddressId = addressId;
            IsGeneralQuoteRequest = isGeneralQuoteRequest;
            IsShortQuote = isShortQuote;
            PortfolioItemId = portfolioItemId;
            Quantity = quantity;

            if (attachmentUrls != null)
                AttachmentUrls.AddRange(attachmentUrls);
        }
    }
}
