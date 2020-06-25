using Application.Config;
using Domain.Core;
using Domain.Entities;
using Domain.Events;
using Domain.Events.QuoteRequests;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Framework;
using Microsoft.AspNet.Identity;
using System.Web;
using V2.Application.Messages;
using V2.Application.Modules.QuoteRequests.Messages;
using V2.Application.Modules.QuoteRequests.Messages.Specs;

namespace V2.Application.Modules.QuoteRequests.EventHandlers
{
    public class SendEmailToBuyerConfirmingQuoteGroupRequestSentEventHandler : IDomainEventHandler<NewQuoteGroupRegisteredEvent>
    {
        private readonly IEmailSender _messageSender;
        private readonly IPersonRepository _personRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SendEmailToBuyerConfirmingQuoteGroupRequestSentEventHandler(
            IEmailSender messageSender,
            IPersonRepository personRepository,
            UserManager<ApplicationUser> userManager)
        {
            _messageSender = messageSender;
            _personRepository = personRepository;
            _userManager = userManager;
        }

        public void Handle(NewQuoteGroupRegisteredEvent domainEvent)
        {
            var buildMessageResult = BuildMessage(domainEvent);
            if (!buildMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on BuildMessage. Details: " + buildMessageResult.Errors.ToSimpleString());

            var sendMessageResult = _messageSender.Send(buildMessageResult.Result);
            if (!sendMessageResult.OK) throw new DomainEventHandlerFailedException(GetType(), domainEvent,
                "Failed on SendMessage. Details: " + buildMessageResult.Errors.ToSimpleString());
        }

        private OperationResult<EmailMessage> BuildMessage(NewQuoteGroupRegisteredEvent domainEvent)
        {
            var buyer = _personRepository.GetById(domainEvent.BuyerId);

            // Optimize this; generates a password recovery code (and records it in the database) everytime a quote request is sent.
            var code = _userManager.GeneratePasswordResetToken(buyer.UserId);
            var encoded = HttpUtility.UrlEncode(code);
            var setPasswordUrl = MaaSConfig.Current.Environment.AppUrl + $"account/resetpassword/?userId={buyer.Id}&code={encoded}";

            var model = new NewQuoteGroupMessageToBuyerModel
            {
                GroupId = domainEvent.GroupId,
                BuyerName = buyer.Name,
                QuoteTitle = domainEvent.Title,
                SetPasswordUrl = setPasswordUrl
            };

            return MessagesContext.Current.Generate(new NewQuoteGroupMessageToBuyerSpecification(), model)
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