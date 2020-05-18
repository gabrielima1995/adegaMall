using Application.Config;
using Domain.Events;
using Domain.Interfaces.Repositories;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.Sellers.EventHandlers
{
    public class NotifyAdminForModerationWhenSellerIsRegistered : IDomainEventHandler<SellerRegisteredEvent>
    {
        private readonly IPersonRepository _personRepository;

        public NotifyAdminForModerationWhenSellerIsRegistered(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public void Handle(SellerRegisteredEvent domainEvent)
        {
            if (!MaaSConfig.Current.Seller.Moderation.Enabled) return;

            var admins = _personRepository.ListAdminPersons();

            if (!admins.Any())
                return;

            var messageModel = new AwaitingModerationMessageModel()
            {
                UserName = domainEvent.SellerPerson.FullName,
                Url = $"{MaaSConfig.Current.Environment.AppUrl}admin/sellers/details/{domainEvent.SellerPerson.Id}"
            };

            MessagesContext.Current.GenerateAndSend<AwaitingModerationMessageSpecification, AwaitingModerationMessageModel>(
                messageModel, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress, admins.Select(x => x.Email));
        }
    }
}
