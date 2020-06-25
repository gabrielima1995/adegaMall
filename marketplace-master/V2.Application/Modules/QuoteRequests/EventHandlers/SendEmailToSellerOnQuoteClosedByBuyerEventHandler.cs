using Application.Config;
using Domain.Core;
using Domain.Events;
using Domain.Events.QuoteRequests;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Framework;
using Infra.Crosscutting.Globalization;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.QuoteRequests.Messages;
using V2.Application.Modules.QuoteRequests.Messages.Specs;

namespace V2.Application.Modules.QuoteRequests.EventHandlers
{
    public class SendEmailToSellerOnQuoteClosedByBuyerEventHandler : IDomainEventHandler<QuoteClosedByBuyerEvent>
    {
        private readonly IEmailSender _messageSender;
        private readonly IPersonRepository _personRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IPushNotificationService _notificationService;

        public SendEmailToSellerOnQuoteClosedByBuyerEventHandler(
            IEmailSender messageSender,
            IPersonRepository personRepository,
            IQuoteRepository quoteRepository, IPushNotificationService notificationService)
        {
            _messageSender = messageSender;
            _personRepository = personRepository;
            _quoteRepository = quoteRepository;
            _notificationService = notificationService;
        }

        public void Handle(QuoteClosedByBuyerEvent domainEvent)
        {
            var buildMessageResult = BuildMessage(domainEvent);
            if (!buildMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on BuildMessage. Details: " + buildMessageResult.Errors.ToSimpleString());

            var sendMessageResult = _messageSender.Send(buildMessageResult.Result);
            if (!sendMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on SendMessage. Details: " + buildMessageResult.Errors.ToSimpleString());
        }

        private OperationResult<EmailMessage> BuildMessage(QuoteClosedByBuyerEvent domainEvent)
        {
            var quote = _quoteRepository.GetById(domainEvent.QuoteId);
            var buyer = _personRepository.GetById(quote.Buyer_Id);
            var seller = _personRepository.GetById(quote.Seller_Id);

            //Push notification
            if (quote.DealResult.HasValue && quote.DealResult.Value == Domain.QuoteRequests.QuoteDealResult.Won)
            {
                _notificationService.NotifyUser(seller.UserId, new PushNotification()
                {
                    Title = GlobalizationHelper.GetString("NOTIFICATION_QUOTE_ACCEPTED_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                    Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_QUOTE_ACCEPTED_DESC", MaaSConfig.Current.Localization.DefaultLang), buyer.FullName.Split(' ').First(x => x.Length > 0)),
                    Link = $"/quotes/details/{domainEvent.QuoteId}",
                });
            }
            else if (quote.DealResult.HasValue && quote.DealResult.Value == Domain.QuoteRequests.QuoteDealResult.Lost)
            {
                _notificationService.NotifyUser(seller.UserId, new PushNotification()
                {
                    Title = GlobalizationHelper.GetString("NOTIFICATION_QUOTE_CANCELLED_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                    Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_QUOTE_CANCELLED_DESC", MaaSConfig.Current.Localization.DefaultLang), buyer.FullName.Split(' ').First(x => x.Length > 0)),
                    Link = $"/quotes/details/{domainEvent.QuoteId}",
                });
            }

            var model = new QuoteMessageModel
            {
                BuyerName = buyer.Name,
                SellerName = seller.AlternativeName ?? seller.Name,
                QuoteId = quote.Id,
                QuoteTitle = quote.Title,
                QuoteContent = $"{quote.Title} - {quote.Description}"
            };

            model.Answer = quote?.LatestAnswer?.Text ?? "";

            if (quote.LatestAnswer != null)
            {
                if (quote.LatestAnswer.Price.HasValue)
                    model.Answer += $"  Preço: {quote.LatestAnswer.Price.Value}";

                if (quote.LatestAnswer.DeliveryDeadline.HasValue)
                    model.Answer += $"  Prazo: {quote.LatestAnswer.DeliveryDeadline.Value.ToShortDateString()}";
            }

            var quoteAccepted = quote.DealResult.Value == Domain.QuoteRequests.QuoteDealResult.Won;
            var spec = quoteAccepted
                ? new NewQuoteAcceptedMessageToSellerSpecification() as MessageSpecification<QuoteMessageModel>
                : new NewQuoteClosedMessageToSellerSpecification();

            return MessagesContext.Current.Generate(spec, model)
                .Then(message =>
                {
                    message
                        .ToAddress(seller.Email)
                        .FromAddress(MaaSConfig.Current.Environment.Smtp.DefaultFromAddress);

                    return new OperationResult<EmailMessage>(message);
                });
        }
    }
}