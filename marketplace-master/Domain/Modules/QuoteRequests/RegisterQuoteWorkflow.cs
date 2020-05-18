using Application.Config;
using Domain.Entities.PortfolioModule;
using Domain.Events.QuoteRequests;
using Domain.Interfaces.Repositories;
using Domain.QuoteRequests.Commands;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Linq;

namespace Domain.Modules.Quotes
{
    internal class RegisterQuoteWorkflow
    {
        private readonly MaaSConfig config = MaaSConfig.Current;
        private readonly IQuoteRepository quoteRepository;
        private readonly IUnitOfWork unitOfWork;

        public RegisterQuoteWorkflow(IQuoteRepository quoteRepository, IUnitOfWork unitOfWork)
        {
            this.quoteRepository = quoteRepository;
            this.unitOfWork = unitOfWork;
        }

        public OperationResult<RegisterQuoteResult> RegisterQuoteRequest(RequestQuoteCommand command)
        {
            StringHelper.Sanitize(command);
            var groupId = GenerateGroupIdOrNull(command);

            var addResult = command.SellerIds.Any()
                ? TryAddQuoteWithMultipleSellers(command, groupId)
                : TryAddQuoteWithoutSellers(command, groupId);
            if (addResult.IsFailure) return addResult.Errors;

            var saveResult = unitOfWork.Save();
            if (saveResult.IsFailure) return saveResult.Errors;

            var resultData = command.IsGeneralQuoteRequest
                ? RegisterQuoteResult.OfGroup(groupId.Value)
                : RegisterQuoteResult.OfSingle(command.QuoteId);

            return new OperationResult<RegisterQuoteResult>(resultData);
        }

        private OperationResult TryAddQuoteWithoutSellers(RequestQuoteCommand command, Guid? groupId)
        {
            if (!config.Quote.GeneralQuote.RegisterQuoteEvenWhenNoSellers)
                return new ErrorDictionary("", GlobalizationHelper.GetString("QUOTE_NO_SELLERS_ERROR_MESSAGE"));

            var quote = CreateQuote(command, command.QuoteId, groupId, null);

            if (quote.GroupId.HasValue)
                quote.AddDomainEvent(new NewQuoteGroupRegisteredEvent(quote.GroupId.Value, quote.Buyer_Id, quote.Title, hasSeller: false));

            return quoteRepository.Add(quote);
        }

        private OperationResult TryAddQuoteWithMultipleSellers(RequestQuoteCommand command, Guid? groupId)
        {
            var quoteId = command.QuoteId;
            var groupEventAdded = false;

            foreach (var sellerId in command.SellerIds)
            {
                var quote = CreateQuote(command, quoteId, groupId, sellerId);

                if (quote.GroupId.HasValue && !groupEventAdded)
                {
                    quote.AddDomainEvent(new NewQuoteGroupRegisteredEvent(quote.GroupId.Value, quote.Buyer_Id, quote.Title));
                    groupEventAdded = true;
                }

                var result = quoteRepository.Add(quote);
                if (result.IsFailure) return result;

                quoteId = Guid.NewGuid();
            }

            return OperationResult.Success;
        }

        private Quote CreateQuote(RequestQuoteCommand command, Guid quoteId, Guid? groupId, string sellerId)
        {
            return new Quote(
                quoteId, command.Title, command.Description,
                command.Phone, command.Date, command.Preference,
                command.Amount, groupId, command.CategoryId,
                command.SubCategoryId, command.BuyerId, sellerId,
                command.AddressId, command.AttachmentUrls, command.IsShortQuote,
                command.PortfolioItemId, command.Quantity);
        }

        private Guid? GenerateGroupIdOrNull(RequestQuoteCommand command) => command.IsGeneralQuoteRequest ? Guid.NewGuid() : (Guid?)null;
    }

    public class RegisterQuoteResult
    {
        public Guid GroupId { get; private set; }
        public Guid SingleQuoteId { get; private set; }
        public bool IsGroup => GroupId != Guid.Empty;
        public static RegisterQuoteResult OfGroup(Guid groupId) => new RegisterQuoteResult { GroupId = groupId };
        public static RegisterQuoteResult OfSingle(Guid quoteId) => new RegisterQuoteResult { SingleQuoteId = quoteId };
    }
}