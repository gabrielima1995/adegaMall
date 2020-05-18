using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public interface IWithCustomFields
    {
        CustomFieldEntityType EntityType { get; }

        Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }

        Dictionary<string, string> CustomFields { get; set; }

        Guid Id { get; set; }
    }

    public static class WithCustomFieldsExtensions
    {
        public static bool HasCustomFields(this IWithCustomFields obj)
        {
            return obj.CustomFields != null && obj.CustomFields.Count > 0;
        }
    }
}
