using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Config;
using Domain.Events;
using Domain.Interfaces.Repositories;
using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.Buyers.EventHandlers
{
    public class NotifyAdminForModerationWhenBuyerIsRegistered : IDomainEventHandler<BuyerRegisteredEvent>
    {
        readonly IPersonRepository _personRepository;

        public NotifyAdminForModerationWhenBuyerIsRegistered(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public void Handle(BuyerRegisteredEvent domainEvent)
        {
            if (!MaaSConfig.Current.Buyer.Moderation.Enabled) return;

            var admins = _personRepository.ListAdminPersons();

            if (!admins.Any())
                return;

            var messageModel = new AwaitingModerationBuyerMessageModel()
            {
                UserName = domainEvent.Buyer.FullName,
                Url = $"{MaaSConfig.Current.Environment.AppUrl}admin/buyers/details/{domainEvent.Buyer.Id}"
            };

            MessagesContext.Current.GenerateAndSend<AwaitingModerationBuyerMessageSpecification, AwaitingModerationBuyerMessageModel>(
                messageModel, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress, admins.Select(x => x.Email));
        }
    }
}
