using Application.Config;
using Domain.Core;
using Domain.Entities;
using Domain.Events;
using Domain.Events.QuoteBillingModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Framework;
using Infra.Crosscutting.Globalization;
using Microsoft.AspNet.Identity;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.QuoteRequests.Messages;
using V2.Application.Modules.QuoteRequests.Messages.Specs;

namespace V2.Application.Modules.QuoteRequests.EventHandlers
{
    public class SendEmailToBuyerOnQuoteBillingCreatedEventHandler : IDomainEventHandler<QuoteBillingCreatedEvent>
    {
        private readonly IEmailSender _messageSender;
        private readonly IPersonRepository _personRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IPushNotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SendEmailToBuyerOnQuoteBillingCreatedEventHandler(
            IEmailSender messageSender,
            IPersonRepository personRepository,
            IQuoteRepository quoteRepository,
            UserManager<ApplicationUser> userManager, IPushNotificationService notificationService)
        {
            _messageSender = messageSender;
            _personRepository = personRepository;
            _quoteRepository = quoteRepository;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public void Handle(QuoteBillingCreatedEvent domainEvent)
        {
            var buildMessageResult = BuildMessage(domainEvent);
            if (!buildMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on BuildMessage. Details: " + buildMessageResult.Errors.ToSimpleString());

            var sendMessageResult = _messageSender.Send(buildMessageResult.Result);
            if (!sendMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on SendMessage. Details: " + buildMessageResult.Errors.ToSimpleString());
        }

        private OperationResult<EmailMessage> BuildMessage(QuoteBillingCreatedEvent domainEvent)
        {
            var buyer = _personRepository.GetById(domainEvent.QuoteBilling.BuyerId);
            var quote = _quoteRepository.GetById(domainEvent.QuoteBilling.QuoteId);

            //Send push notification
            _notificationService.NotifyUser(buyer.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_QUOTE_BILLING_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                Body = quote.Title,
                Link = $"quotes/pay/?billingId={domainEvent.QuoteBilling.Id.ToString()}",
            });
            //

            var model = new QuoteBillingCreatedMessageModel()
            {
                QuoteId = domainEvent.QuoteBilling.QuoteId,
                BuyerName = buyer.Name,
                QuoteTitle = quote.Title,
                BillingUrl = MaaSConfig.Current.Environment.AppUrl + "quotes/pay/?billingId=" + domainEvent.QuoteBilling.Id.ToString(),
            };

            return MessagesContext.Current.Generate(new NewQuoteBillingMessageToBuyerSpecification(), model)
                .Then(message =>
                {
                    message
                        .ToAddress(buyer.Email)
                        .FromAddress(MaaSConfig.Current.Environment.Smtp.DefaultFromAddress);

                    return new OperationResult<EmailMessage>(message);
                });
        }
    }
}