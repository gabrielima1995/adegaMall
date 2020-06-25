using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CustomField : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Name_EN { get; set; }

        public string Placeholder { get; set; }
        public string Placeholder_EN { get; set; }

        public int Order { get; set; }

        public CustomFieldDataType DataType { get; private set; }
        public CustomFieldEntityType EntityType { get; private set; }
        public CustomFieldInputType InputType { get; private set; }

        public List<CustomFieldValue> Values { get; private set; } = new List<CustomFieldValue>();
        public List<CustomFieldOption> Options { get; private set; } = new List<CustomFieldOption>();

        public bool IsPublic { get; set; }
        public bool IsMandatory { get; set; }
        public bool BlockChangesAfterApprovedInModeration { get; set; }

        public CustomField()
        {
            Id = Guid.NewGuid();
        }

        public CustomField(CustomFieldDataType dataType, CustomFieldEntityType entityType, CustomFieldInputType inputType)
        {
            Id = Guid.NewGuid();
            DataType = dataType;
            EntityType = entityType;
            InputType = inputType;
        }

        public string NameLocalized(string lang)
        {
            if (string.Equals(lang, "en-us", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(Name_EN))
                return Name_EN;

            return Name;
        }

        public string PlaceholderLocalized(string lang)
        {
            if (string.Equals(lang, "en-us", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(Placeholder_EN))
                return Placeholder_EN;

            return Placeholder;
        }
    }

    public class CustomFieldValue : BaseEntity<Guid>
    {
        public Guid EntityId { get; set; }
        public string Value { get; set; }

        public CustomFieldValue()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Returns a CustomFieldValue with null Value for the specified entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public static CustomFieldValue Empty(Guid entityId)
        {
            return new CustomFieldValue() { EntityId = entityId, Value = null };
        }

        public bool HasValue()
        {
            return !string.IsNullOrWhiteSpace(Value);
        }
    }

    public class CustomFieldOption : BaseEntity<Guid>
    {
        public string Name { get; set; }

        public CustomFieldOption()
        {
            Id = Guid.NewGuid();
        }
    }

    public enum CustomFieldDataType
    {
        [Display(Name = "Texto")]
        RawText = 0,

        [Display(Name = "Número")]
        Number = 1,

        [Display(Name = "Data")]
        Date = 2,

        [Display(Name = "Data e Hora")]
        DateTime = 3,

        [Display(Name = "Sim/Não")]
        Boolean = 4,

        [Display(Name = "Arquivo")]
        File = 5,
    }

    public enum CustomFieldEntityType
    {
        Quote,
        Buyer,
        Seller,
        Product,
        Portfolio,
        CartItem
    }

    public enum CustomFieldInputType
    {
        [Display(Name = "Caixa de texto")]
        Text = 0,

        [Display(Name = "Texto multi-linha")]
        MultilineText = 1,

        [Display(Name = "Lista de opções")]
        Options = 2,

        [Display(Name = "Caixa de seleção")]
        Checkbox = 3,

        [Display(Name = "Calendário")]
        DatePicker = 4,

        [Display(Name = "Arquivo")]
        File = 5,
    }
}
