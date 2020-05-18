using System;
using System.Collections.Generic;

namespace V2.Application.Modules.General.CustomFields
{
    public class CustomFieldValueDto
    {
        public CustomFieldValueDto(string entityId, string value, string name)
        {
            EntityId = Guid.Parse(entityId);
            Value = value;
            Name = name;
        }
        public Guid EntityId { get; set; }
        public string Value { get; set; }
        public string Name { get; }
    }
}
