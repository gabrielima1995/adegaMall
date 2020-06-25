using Application.Config;
using Domain.Entities.DisputeModule;
using Domain.Events;
using Domain.Interfaces.Repositories;
using System;
using V2.Application.Messages;
using V2.Application.Modules.Disputes.Messages;
using V2.Application.Modules.Disputes.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    internal class SendEmailToAdminsOnRequestMediation : IDomainEventHandler<DisputeStatusChangedEvent>
    {
        private readonly IPersonRepository personRepository;

        public SendEmailToAdminsOnRequestMediation(IPersonRepository personRepository)
        {
            this.personRepository = personRepository;
        }

        public void Handle(DisputeStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus.NotIn(DisputeStatus.MediationRequestedByBuyer, DisputeStatus.MediationRequestedBySeller))
                return;

            var admins = personRepository.ListAdminPersons();
            foreach (var admin in admins)
            {
                MessagesContext.Current.GenerateAndSend<DisputeMediationRequestMessageToAdminsSpecification, DisputeMessageModel>(
                    new DisputeMessageModel()
                    {
                        RecipientName = admin.FullName,
                        OrderId = new Guid(domainEvent.Dispute.OrderId),
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { admin.Email });
            }
        }
    }
}
