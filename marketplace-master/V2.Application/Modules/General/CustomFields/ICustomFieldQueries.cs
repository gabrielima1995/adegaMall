using Domain.Entities;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.General.CustomFields
{
    public interface ICustomFieldQueries
    {
        Dictionary<string, string> ReadValues(CustomFieldEntityType entityType, Guid entityId);

        List<CustomFieldValueDto> ReadValuesForEntityId(CustomFieldEntityType entityType, Guid entityId);
    }
}
