using Application.Config;
using Domain.Core;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Events;
using Domain.Events.QuoteRequests;
using Domain.Interfaces.Repositories;
using Infra.Crosscutting.Globalization;
using System;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.QuoteRequests.Messages;
using V2.Application.Modules.QuoteRequests.Messages.Specs;

namespace V2.Application.Modules.QuoteRequests.EventHandlers
{
    public class SendEmailWhenQuoteAnswerIsAddedEventHandler : IDomainEventHandler<QuoteAnswerAddedEvent>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IPushNotificationService _notificationService;

        public SendEmailWhenQuoteAnswerIsAddedEventHandler(
            IPersonRepository personRepository,
            IQuoteRepository quoteRepository,
            IPushNotificationService notificationService)
        {
            _personRepository = personRepository;
            _quoteRepository = quoteRepository;
            _notificationService = notificationService;
        }

        public void Handle(QuoteAnswerAddedEvent domainEvent)
        {
            var buyer = _personRepository.GetById(domainEvent.BuyerId);
            var seller = _personRepository.GetById(domainEvent.SellerId);
            var quote = _quoteRepository.GetById(domainEvent.QuoteId);

            if (domainEvent.AuthorIsBuyer)
            {
                SendAnswerToSeller(domainEvent, buyer, seller, quote);
            }

            if (domainEvent.AuthorIsSeller)
            {
                SendAnswerToBuyer(domainEvent, buyer, seller, quote);
            }

            if (domainEvent.AuthorIsModerator)
            {
                SendModeratorAnswerToBuyer(domainEvent, buyer, quote);
                SendModeratorAnswerToSeller(domainEvent, seller, quote);
            }
        }

        private string GetAnswer(Quote quote)
        {
            var answer = string.Empty;

            if (quote.LatestAnswer != null)
            {
                answer = quote.LatestAnswer.Text;

                if (quote.LatestAnswer.Price.HasValue)
                    answer += $"  Preço: {quote.LatestAnswer.Price.Value}";

                if (quote.LatestAnswer.DeliveryDeadline.HasValue)
                    answer += $"  Prazo: {quote.LatestAnswer.DeliveryDeadline.Value.ToShortDateString()}";
            }

            return answer;
        }

        private QuoteMessageModel CreateMessage(Person buyer, Person seller, Quote quote)
        {
            var model = new QuoteMessageModel
            {
                BuyerName = buyer.Name,
                SellerName = seller.AlternativeName ?? seller.Name,
                QuoteId = quote.Id,
                QuoteTitle = quote.Title,
                Answer = GetAnswer(quote)
            };

            return model;
        }

        private QuoteModeratorMessageModel CreateModeratorMessage(Person target, Quote quote)
        {
            var model = new QuoteModeratorMessageModel
            {
                TargetName = target.Name,
                QuoteId = quote.Id,
                QuoteTitle = quote.Title,
                Answer = GetAnswer(quote)
            };

            return model;
        }

        private void NotifyUser(string targetId, string senderName, Quote quote)
        {
            _notificationService.NotifyUser(targetId, new PushNotification
            {
                Title = string.Format(GlobalizationHelper.GetString("NOTIFICATION_QUOTE_MESSAGE_TITLE", MaaSConfig.Current.Localization.DefaultLang), senderName),
                Body = quote.LatestAnswer.Text,
                Link = $"/quotes/details/{quote.Id.ToString()}"
            });
        }

        private void SendAnswerToBuyer(QuoteAnswerAddedEvent domainEvent, Person buyer, Person seller, Quote quote)
        {
            NotifyUser(buyer.UserId, "Vendedor", quote);

            var model = CreateMessage(buyer, seller, quote);

            var result = MessagesContext.Current
                .GenerateAndSend<NewQuoteAnswerMessageToBuyerSpecification, QuoteMessageModel>(
                    model,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { buyer.Email });

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }

        private void SendModeratorAnswerToBuyer(QuoteAnswerAddedEvent domainEvent, Person buyer, Quote quote)
        {
            NotifyUser(buyer.UserId, "Moderador", quote);

            var model = CreateModeratorMessage(buyer, quote);

            var result = MessagesContext.Current
                .GenerateAndSend<NewModeratorQuoteAnswerMessageToTargetSpecification, QuoteModeratorMessageModel>(
                    model,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { buyer.Email });

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }

        private void SendAnswerToSeller(QuoteAnswerAddedEvent domainEvent, Person buyer, Person seller, Quote quote)
        {
            NotifyUser(seller.UserId, buyer.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).First(), quote);

            var model = CreateMessage(buyer, seller, quote);

            var result = MessagesContext.Current
                .GenerateAndSend<NewQuoteAnswerMessageToSellerSpecification, QuoteMessageModel>(
                    model,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { seller.Email });

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }

        private void SendModeratorAnswerToSeller(QuoteAnswerAddedEvent domainEvent, Person seller, Quote quote)
        {
            NotifyUser(seller.UserId, "Moderador", quote);

            var model = CreateModeratorMessage(seller, quote);

            var result = MessagesContext.Current
                .GenerateAndSend<NewModeratorQuoteAnswerMessageToTargetSpecification, QuoteModeratorMessageModel>(
                    model,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { seller.Email });

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }
    }
}