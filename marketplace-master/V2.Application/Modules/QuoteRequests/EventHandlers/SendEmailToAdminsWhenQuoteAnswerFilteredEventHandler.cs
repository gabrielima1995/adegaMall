using Application.Config;
using Domain.Core;
using Domain.Events;
using Domain.Events.QuoteRequests;
using Domain.Interfaces.Repositories;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.QuoteRequests.Messages;
using V2.Application.Modules.QuoteRequests.Messages.Specs;

namespace V2.Application.Modules.QuoteRequests.EventHandlers
{
    public class SendEmailToAdminsWhenQuoteAnswerFilteredEventHandler : IDomainEventHandler<QuoteAnswerFilteredEvent>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IQuoteRepository _quoteRepository;

        public SendEmailToAdminsWhenQuoteAnswerFilteredEventHandler(
            IPersonRepository personRepository,
            IQuoteRepository quoteRepository)
        {
            _personRepository = personRepository;
            _quoteRepository = quoteRepository;
        }

        public void Handle(QuoteAnswerFilteredEvent domainEvent)
        {
            var admins = _personRepository.ListAdminPersons();

            var buyer = _personRepository.GetById(domainEvent.BuyerId);
            var seller = _personRepository.GetById(domainEvent.SellerId);
            var quote = _quoteRepository.GetById(domainEvent.QuoteId);

            var model = new QuoteAnswerFilteredMessageModel()
            {
                Answer = domainEvent.Message,
                BuyerName = buyer.Name,
                SellerName = seller.AlternativeName ?? seller.Name,
                QuoteId = domainEvent.QuoteId,
                QuoteTitle = quote.Title,
            };

            var result = MessagesContext.Current.GenerateAndSend<QuoteAnswerFilteredMessageToAdminSpecification, QuoteAnswerFilteredMessageModel>(
                model,
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                admins.Select(x => x.Email));

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }
    }
}