using Application.Config;
using Domain.Core;
using Domain.Events.QuoteRequests;
using Domain.Modules.Portfolio;
using Domain.QuoteRequests;
using Domain.Utils;
using FluentValidation;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.PortfolioModule
{
    public class Quote : BaseEntity<Guid>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Phone { get; set; }

        public DateTime? Date { get; set; }

        public QuotePreference Preference { get; set; }

        public decimal? Amount { get; set; }

        public QuoteStatus Status { get; set; }

        public Guid? GroupId { get; set; }

        public int? Category_Id { get; set; }

        public int? SubCategory_Id { get; set; }

        public string Buyer_Id { get; set; }

        public string Seller_Id { get; set; }

        public Guid Address_Id { get; set; }

        public Guid? PortfolioItem_Id { get; set; }

        [ForeignKey("Category_Id")]
        public Category Category { get; set; }

        [ForeignKey("SubCategory_Id")]
        public SubCategory SubCategory { get; set; }

        [ForeignKey("Buyer_Id")]
        public Buyer Buyer { get; set; }

        [ForeignKey("Seller_Id")]
        public Seller Seller { get; set; }

        [ForeignKey("Address_Id")]
        public Address Address { get; set; }

        [ForeignKey("PortfolioItem_Id")]
        public PortfolioItem PortfolioItem { get; set; }

        public int Quantity { get; set; }

        public List<QuoteAnswer> Answers { get; set; } = new List<QuoteAnswer>();

        public QuoteAnswer LatestAnswer { get; set; }

        public bool IsClosed { get; private set; }

        public DateTime? ClosedAt { get; private set; }

        public QuoteDealResult? DealResult { get; private set; }

        public List<QuoteAttachment> Attachments { get; set; } = new List<QuoteAttachment>();

        public DateTime? Deadline { get; set; }

        public decimal? AcceptedPrice { get; set; }

        [NotMapped]
        public bool HasAnswerFromSeller { get; set; }

        [NotMapped]
        public bool IsGeneralQuoteWithoutSeller => GroupId.HasValue && Seller_Id == null;

        public bool IsShortQuote { get; set; }

        public bool IsPortfolioQuote => PortfolioItem_Id.HasValue;

        public Quote()
        {
        }

        public Quote(
            Guid id, string title, string description, string phone, DateTime? date,
            QuotePreference preference, decimal? amount, Guid? groupId,
            int? categoryId, int? subCategoryId, string buyerId, string sellerId, Guid addressId,
            IEnumerable<string> attachmentUrls, bool isShortQuote,
            Guid? portfolioItemId, int quantity)
        {
            Id = id;

            Title = title;
            Description = description;
            Phone = phone;
            Date = date;

            Preference = preference;
            Amount = amount;
            GroupId = groupId;

            Category_Id = categoryId;
            SubCategory_Id = subCategoryId;
            Buyer_Id = buyerId;
            Seller_Id = sellerId;
            Address_Id = addressId;
            PortfolioItem_Id = portfolioItemId;
            Quantity = quantity;
            IsShortQuote = isShortQuote;

            Status = QuoteStatus.Opened;

            if (attachmentUrls != null)
            {
                foreach (var url in attachmentUrls)
                {
                    var attachment = new QuoteAttachment(Guid.NewGuid(), Id, null, url);
                    Attachments.Add(attachment);
                }
            }

            if (MaaSConfig.Current.Quote.AnswersFilterContactInfo)
            {
                var filterResult = new ContactInfoFilter().Filter(description);

                if (filterResult.FoundAnyMatches)
                    AddDomainEvent(new QuoteAnswerFilteredEvent(Id, Seller_Id, Buyer_Id, Buyer_Id, description));

                Description = filterResult.FilteredText;
            }

            CheckInvariants();
            AddDomainEvent(new NewQuoteRegisteredEvent(Id, Buyer_Id, Seller_Id, Title, GroupId));
        }

        public OperationResult CanAddAnswer(string authorId, string text, decimal? price, DateTime? deadline, IEnumerable<string> attachmentUrls)
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrWhiteSpace(text))
                errors.Add(nameof(text), GlobalizationHelper.GetString("QUOTE_ANSWER_TEXT_IS_MANDATORY"));

            if (MaaSConfig.Current.Quote.PriceInAnswerRequired && price == null)
                errors.Add(nameof(price), GlobalizationHelper.GetString("QUOTE_ANSWER_PRICE_IS_MANDATORY"));

            if (deadline.HasValue && deadline <= DateTime.Today)
                errors.Add(nameof(deadline), GlobalizationHelper.GetString("DELIVERY_DEADLINE_CANNOT_BE_PAST_DATE"));

            if (errors.HasErrors)
                return new OperationResult(errors);

            return OperationResult.Success;
        }

        public void AddAnswer(string authorId, string text, decimal? price, DateTime? deadline, IEnumerable<string> attachmentUrls, bool isModerator)
        {
            if (CanAddAnswer(authorId, text, price, deadline, attachmentUrls).IsFailure)
                throw new InvalidOperationException("Attempt to call Quote.AddAnswer with invalid parameters. Use Quote.CanAddAnswer to obtain validation errors.");

            if (MaaSConfig.Current.Quote.AnswersFilterContactInfo)
            {
                var filterResult = new ContactInfoFilter().Filter(text);

                if (filterResult.FoundAnyMatches)
                    AddDomainEvent(new QuoteAnswerFilteredEvent(Id, Seller_Id, Buyer_Id, authorId, text));

                text = filterResult.FilteredText;
            }

            var answer = new QuoteAnswer(Guid.NewGuid(), Id, Seller_Id, Buyer_Id, authorId, text, price, deadline);

            LatestAnswer = answer;
            Answers.Add(answer);

            if (attachmentUrls != null)
            {
                foreach (var url in attachmentUrls)
                {
                    var attachment = new QuoteAttachment(Guid.NewGuid(), Id, answer.Id, url);
                    Attachments.Add(attachment);
                }
            }

            AddDomainEvent(new QuoteAnswerAddedEvent(Id, answer.Id, Seller_Id, Buyer_Id, authorId, isModerator));
        }

        public void Close(bool dealWon)
        {
            IsClosed = true;
            ClosedAt = DateTime.UtcNow;
            DealResult = dealWon ? QuoteDealResult.Won : QuoteDealResult.Lost;
            Status = QuoteStatus.Archived;

            CheckInvariants();
            AddDomainEvent(new QuoteClosedEvent(Id, dealWon));
        }

        public void CheckInvariants()
        {
            var result = new QuoteValidator().Validate(this);
            if (!result.IsValid) throw new InvalidEntityException(GetType(), result);
        }
    }

    internal class QuoteValidator : AbstractValidator<Quote>
    {
        public QuoteValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.Title)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.GroupId)
                .NotEqual(x => Guid.Empty);

            RuleFor(x => x.Category_Id)
                .NotEmpty()
                .Unless(i => i.IsShortQuote || i.IsPortfolioQuote);

            RuleFor(x => x.SubCategory_Id)
                .NotEmpty()
                .Unless(i => i.IsShortQuote || i.IsPortfolioQuote);

            RuleFor(x => x.Address_Id)
                .NotEmpty()
                .Unless(i => i.IsShortQuote);

            RuleFor(x => x.Buyer_Id)
                .NotEmpty();

            RuleFor(x => x.Seller_Id)
                .NotEmpty()
                .Unless(x => MaaSConfig.Current.Quote.GeneralQuote.RegisterQuoteEvenWhenNoSellers);

            RuleFor(x => x.ClosedAt)
                .NotNull()
                .Unless(x => !x.IsClosed);

            RuleFor(x => x.DealResult)
                .NotNull()
                .Unless(x => !x.IsClosed);
        }
    }
}