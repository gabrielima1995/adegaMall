using Domain.Entities;
using Framework;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IMessageTemplateRepository
    {
        MessageTemplate Get(string key, string lang);

        OperationResult Add(MessageTemplate entity, bool saveChanges = false);

        OperationResult Update(MessageTemplate entity, bool saveChanges = false);

        List<MessageTemplate> List();
    }
}