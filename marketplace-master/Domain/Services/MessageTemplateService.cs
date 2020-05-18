using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;

namespace Domain.Services
{
    public class MessageTemplateService
    {
        private readonly IMessageTemplateRepository _messageTemplateRepository;

        public MessageTemplateService(
            IMessageTemplateRepository messageTemplateRepository)
        {
            _messageTemplateRepository = messageTemplateRepository;
        }

        public OperationResult<MessageTemplate> Get(string key, string lang)
        {
            var entity = _messageTemplateRepository.Get(key, lang);

            if (entity == null)
            {
                return OperationResult<MessageTemplate>.Fail("", "Entity not found.");
            }

            return entity;
        }

        public OperationResult<MessageTemplate> Create(MessageTemplateCreating model)
        {
            return MessageTemplate
                .Create(model)
                .Then(result => _messageTemplateRepository
                    .Add(result, true)
                    .Then<MessageTemplate>(() => result));
        }

        public OperationResult<MessageTemplate> Update(MessageTemplateEditing model)
        {
            return Get(model.Key, model.Lang)
                .Then(result => result.Update(model))
                .Then(result => _messageTemplateRepository
                    .Update(result, true)
                    .Then<MessageTemplate>(() => result));
        }
    }
}