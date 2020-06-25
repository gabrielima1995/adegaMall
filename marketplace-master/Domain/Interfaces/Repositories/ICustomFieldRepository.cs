using Domain.Entities;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface ICustomFieldRepository
    {
        OperationResult SetValues(Guid entityId, Dictionary<string, string> values);

        OperationResult SetValuesForAllQuotesInGroup(Guid groupId, Dictionary<string, string> fieldIdsAndValues);

        OperationResult Add(CustomField entity);

        OperationResult Delete(Guid fieldId);

        OperationResult DeleteForEntityId(Guid entityId);

        OperationResult Update(CustomField entity);

        OperationResult DeleteOption(CustomField entity, CustomFieldOption option);

        CustomField GetById(string fieldId);

        List<CustomFieldValue> GetCustomFieldValueByEntityId(string entityId);

        OperationResult UpdateCustomFieldValue(CustomFieldValue value);

        List<CustomField> ListByEntityType(CustomFieldEntityType entityType);

        Dictionary<Guid, CustomField> DictionaryByEntityType(CustomFieldEntityType entityType);

        OperationResult CloneValues(Guid fromEntityId, Guid toEntityId);
    }
}
