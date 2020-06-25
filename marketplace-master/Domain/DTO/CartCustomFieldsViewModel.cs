using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Domain.DTO
{
    public class CartCustomFieldsViewModel : IWithCustomFields
    {
        public CustomFieldEntityType EntityType => CustomFieldEntityType.CartItem;

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();
        public Guid Id { get; set; }
    }
}
