using Application.Config;
using Domain.Core;
using Domain.Events;
using Domain.Events.QuoteRequests;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.QuoteRequests.Messages;
using V2.Application.Modules.QuoteRequests.Messages.Specs;

namespace V2.Application.Modules.QuoteRequests.EventHandlers
{
    public class SendEmailToSellerOnNewQuoteRequestEventHandler : IDomainEventHandler<NewQuoteRegisteredEvent>
    {
        private static readonly List<Guid> processedGroupIds = new List<Guid>();

        private readonly IEmailSender messageSender;
        private readonly IPushNotificationService notificationService;
        private readonly IPersonRepository personRepository;
        private readonly IQuoteRepository quoteRepository;

        public SendEmailToSellerOnNewQuoteRequestEventHandler(
            IEmailSender messageSender,
            IPushNotificationService notificationService,
            IPersonRepository personRepository,
            IQuoteRepository quoteRepository)
        {
            this.messageSender = messageSender;
            this.personRepository = personRepository;
            this.quoteRepository = quoteRepository;
            this.notificationService = notificationService;
        }

        public void Handle(NewQuoteRegisteredEvent domainEvent)
        {
            if (domainEvent.SellerId == null)
                return;

            var seller = personRepository.GetById(domainEvent.SellerId);

            notificationService.NotifyUser(seller.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_NEW_QUOTE_TITLE", Lang.pt_BR),
                Body = domainEvent.Title,
                Link = $"/quotes/details/{domainEvent.QuoteId}",
            });

            var buildMessageResult = BuildMessage(domainEvent);
            if (!buildMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on BuildMessage. Details: " + buildMessageResult.Errors.ToSimpleString());

            var sendMessageResult = messageSender.Send(buildMessageResult.Result);
            if (!sendMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on SendMessage. Details: " + sendMessageResult.Errors.ToSimpleString());

            if (MaaSConfig.Current.Quote.SendQuoteCopyToAdmins)
            {
                var adminResult = SendCopyToAdmin(domainEvent);
                if (!adminResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                 "Failed on SendCopy. Details: " + adminResult.Errors.ToSimpleString());
            }
        }

        private OperationResult SendCopyToAdmin(NewQuoteRegisteredEvent domainEvent)
        {
            if (CopyToAdminAlreadySent(domainEvent.GroupId))
                return OperationResult.Success;

            RememberCopyToAdminAlreadySent(domainEvent.GroupId);

            var buyer = personRepository.GetById(domainEvent.BuyerId);
            var seller = personRepository.GetById(domainEvent.SellerId);
            var admins = personRepository.ListAdminPersons();
            var quote = quoteRepository.GetById(domainEvent.QuoteId);

            var model = new QuoteMessageModel
            {
                BuyerName = buyer.Name,
                SellerName = domainEvent.GroupId.HasValue ? "múltiplos vendedores" : seller.AlternativeName ?? seller.Name,
                QuoteId = domainEvent.QuoteId,
                QuoteTitle = domainEvent.Title,
                QuoteContent = $"{quote.Title} - {quote.Description}",
            };

            return MessagesContext.Current.GenerateAndSend<NewQuoteMessageToAdminSpecification, QuoteMessageModel>(
                model, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress, admins.Select(x => x.Email));
        }

        private OperationResult<EmailMessage> BuildMessage(NewQuoteRegisteredEvent domainEvent)
        {
            var buyer = personRepository.GetById(domainEvent.BuyerId);
            var seller = personRepository.GetById(domainEvent.SellerId);

            var model = new QuoteMessageModel
            {
                BuyerName = buyer.Name,
                SellerName = seller.AlternativeName ?? seller.Name,
                QuoteId = domainEvent.QuoteId,
                QuoteTitle = domainEvent.Title
            };

            return MessagesContext.Current.Generate(new NewQuoteMessageToSellerSpecification(), model)
                .Then(message =>
                {
                    message
                        .ToAddress(seller.Email)
                        .FromAddress(MaaSConfig.Current.Environment.Smtp.DefaultFromAddress);

                    return new OperationResult<EmailMessage>(message);
                });
        }

        private bool CopyToAdminAlreadySent(Guid? groupId)
        {
            return groupId.HasValue && processedGroupIds.Contains(groupId.Value);
        }

        private void RememberCopyToAdminAlreadySent(Guid? groupId)
        {
            if (groupId.HasValue)
                processedGroupIds.Add(groupId.Value);
        }
    }
}