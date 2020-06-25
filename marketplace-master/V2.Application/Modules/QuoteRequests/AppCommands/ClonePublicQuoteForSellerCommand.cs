using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Events.QuoteRequests;
using Domain.Interfaces.Repositories;
using Framework;
using System;

namespace V2.Application.Modules.QuoteRequests.AppCommands
{
    public class ClonePublicQuoteForSellerCommand
    {
        public ClonePublicQuoteForSellerCommand(Guid groupId, Guid newQuoteId, string sellerId, bool treatAsNewQuote = false)
        {
            GroupId = groupId;
            NewQuoteId = newQuoteId;
            SellerId = sellerId ?? throw new ArgumentNullException(nameof(sellerId));
            TreatAsNewQuote = treatAsNewQuote;
        }

        public Guid GroupId { get; }
        public Guid NewQuoteId { get; }
        public string SellerId { get; }
        public bool TreatAsNewQuote { get; }
    }

    public class ClonePublicQuoteForSellerCommandHandler
    {
        private readonly ICustomFieldRepository customFieldRepository;
        private readonly IQuoteRepository quoteRepository;
        private readonly ISellerRepository sellerRepository;

        public ClonePublicQuoteForSellerCommandHandler(ICustomFieldRepository customFieldRepository, IQuoteRepository quoteRepository, ISellerRepository sellerRepository)
        {
            this.customFieldRepository = customFieldRepository;
            this.quoteRepository = quoteRepository;
            this.sellerRepository = sellerRepository;
        }

        public void Handle(ClonePublicQuoteForSellerCommand command)
        {
            var baseQuote = quoteRepository.Get(x => x.GroupId == command.GroupId);
            var seller = sellerRepository.GetById(command.SellerId);

            if (baseQuote == null)
                throw new Exception("Quote not found, groupId = " + command.GroupId);

            if (seller == null)
                throw new Exception("Seller not found, id = " + command.SellerId);

            var clone = CloneQuoteAndSetProperties(baseQuote, command.NewQuoteId, seller, command.TreatAsNewQuote);
            var addResult = quoteRepository.Add(clone, saveChanges: true);

            if (addResult.IsFailure)
                throw new Exception("Error saving quote clone. " + addResult.Errors.ToSimpleString());

            CloneCustomFields(baseQuote.Id, clone.Id);
        }

        private void CloneCustomFields(Guid baseQuoteId, Guid quoteCloneId)
        {
            var cloneResult = customFieldRepository.CloneValues(
                baseQuoteId, quoteCloneId);

            if (cloneResult.IsFailure)
                throw new Exception($"Error cloning custom field values from quote entity {baseQuoteId} to {quoteCloneId}. " +
                    cloneResult.Errors.ToSimpleString());
        }

        private Quote CloneQuoteAndSetProperties(Quote baseQuote, Guid cloneId, Seller seller, bool treatAsNewQuote)
        {
            var clone = new Quote();
            SimpleMapper.Map(baseQuote, clone);

            clone.Id = cloneId;
            clone.Seller_Id = seller.Id;
            clone.Seller = seller;
            clone.Status = QuoteStatus.Opened;
            clone.CreatedAt = DateTime.UtcNow;
            clone.AcceptedPrice = null;
            clone.Deadline = null;

            if (treatAsNewQuote)
                clone.AddDomainEvent(new NewQuoteRegisteredEvent(
                    clone.Id, clone.Buyer_Id, clone.Seller_Id, clone.Title, clone.GroupId));

            return clone;
        }
    }
}
