using Domain.Entities.PortfolioModule;
using Domain.Events.QuoteRequests;
using Domain.Interfaces.Repositories;
using Domain.QuoteRequests.Commands;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;

namespace Domain.Modules.Quotes
{
    public class QuoteService
    {
        private readonly IQuoteRepository quoteRepository;
        private readonly IUnitOfWork unitOfWork;

        public QuoteService(IQuoteRepository quoteRepository, IUnitOfWork unitOfWork)
        {
            this.quoteRepository = quoteRepository;
            this.unitOfWork = unitOfWork;
        }

        public OperationResult AddAnswer(AddQuoteAnswerCommand command)
        {
            StringHelper.Sanitize(command);

            if (command.Deadline.HasValue && command.Deadline.Value < DateTime.Now)
                return OperationResult.Fail(GlobalizationHelper.GetString("QUOTE_ANSWER_CREATE_DATE_TIME_ERROR"));

            var quote = quoteRepository.GetById(command.QuoteId);

            if (quote == null)
                return OperationResult.Fail($"Quote '{command.QuoteId}' not found.");

            var canAddResult = quote.CanAddAnswer(command.AuthorId, command.Text, command.Price, command.Deadline, command.AttachmentUrls);
            if (canAddResult.IsFailure)
                return canAddResult;

            quote.AddAnswer(command.AuthorId, command.Text, command.Price, command.Deadline, command.AttachmentUrls, command.IsModerator);
            return unitOfWork.Save();
        }

        public OperationResult AcceptQuoteAndCloseOthers(AcceptQuoteCommand command)
        {
            var result = new OperationResult();
            var quote = quoteRepository.GetById(command.QuoteId);

            if (quote == null)
                return OperationResult.Fail($"Quote '{command.QuoteId}' not found.");

            if (quote.IsClosed)
                return OperationResult.Success;

            if (command.Deadline < DateTime.Now)
                return OperationResult.Fail(GlobalizationHelper.GetString("QUOTE_ANSWER_ACCEPT_DATE_TIME_ERROR"));

            quote.AcceptedPrice = command.Amount;
            quote.Deadline = command.Deadline;

            quote.Close(true);
            quote.AddDomainEvent(new QuoteClosedByBuyerEvent(command.QuoteId));

            if (quote.GroupId.HasValue)
            {
                var groupQuotes = ListByGroup(quote.GroupId.Value);

                foreach (var q in groupQuotes)
                {
                    if (q.Id == command.QuoteId)
                        continue;

                    var cmd = new CloseQuoteCommand(q.Id, false);
                    var closeResult = CloseQuote(cmd, true);
                    result.Errors.Merge(closeResult.Errors);
                }
            }

            var saveResult = unitOfWork.Save();
            result.Errors.Merge(saveResult.Errors);

            return result;
        }

        public OperationResult CloseQuote(CloseQuoteCommand command, bool isBuyer = false)
        {
            var quote = quoteRepository.GetById(command.QuoteId);
            if (quote == null) return OperationResult.Fail($"Quote '{command.QuoteId}' not found.");
            if (quote.IsClosed) return OperationResult.Success;

            quote.Close(command.DealWon);

            if (isBuyer)
                quote.AddDomainEvent(new QuoteClosedByBuyerEvent(command.QuoteId));

            return unitOfWork.Save();
        }

        public OperationResult<RegisterQuoteResult> RegisterQuoteRequest(RequestQuoteCommand command)
        {
            var workflow = new RegisterQuoteWorkflow(quoteRepository, unitOfWork);
            return workflow.RegisterQuoteRequest(command);
        }

        public Quote GetById(Guid id) => quoteRepository.GetById(id);

        public Quote GetDetails(Guid id) => quoteRepository.GetDetails(id);

        public List<Quote> ListByGroup(Guid groupId) => quoteRepository.GetListByGroup(groupId);

        public List<Quote> List(string buyerId, string sellerId, QuoteStatus? status) => quoteRepository.List(buyerId, sellerId, status);
    }
}