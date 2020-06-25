using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Application.Modules.BatchImport.Models
{
    public class CustomFieldDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Placeholder { get; set; }
        public int Order { get; set; }
        public CustomFieldDataType DataType { get; set; }
        public CustomFieldInputType InputType { get; set; }
        public bool IsMandatory { get; set; }
        public IEnumerable<CustomFieldOptionDto> Options { get; set; } = new CustomFieldOptionDto[0];
    }

    public class CustomFieldOptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomFieldValueDto
    {
        public CustomFieldValueDto(Guid id, object value)
        {
            Id = id;
            Value = value;
        }

        public Guid Id { get; set; }
        public object Value { get; set; }
    }
}
