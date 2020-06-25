using Application.Config;
using Domain.Events;
using Domain.Interfaces.Repositories;
using V2.Application.Messages;

namespace V2.Application.Modules.SellerAssistantModule
{
    public class SendEmailWhenSellerAssistantRegistered : IDomainEventHandler<SellerAssistantRegisteredEvent>
    {
        private readonly IPersonRepository personRepository;

        public SendEmailWhenSellerAssistantRegistered(IPersonRepository personRepository)
        {
            this.personRepository = personRepository;
        }

        public void Handle(SellerAssistantRegisteredEvent domainEvent)
        {
            var seller = personRepository.GetById(domainEvent.Assistant.SellerId);

            var model = new SellerAssistantRegisteredMessageModel
            {
                Name = domainEvent.Assistant.Name,
                Email = domainEvent.Assistant.Email,
                Password = domainEvent.Password,
                SellerName = seller.AlternativeName ?? seller.FullName,
                SellerEmail = seller.Email
            };

            MessagesContext.Current.GenerateAndSend<SellerAssistantRegisteredMessageSpecification, SellerAssistantRegisteredMessageModel>(
                model, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { domainEvent.Assistant.Email }, domainEvent.Assistant.Lang);
        }
    }
}
