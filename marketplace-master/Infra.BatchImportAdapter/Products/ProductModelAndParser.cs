using Application.Modules.BatchImport.Models;
using BatchImport;
using Domain.Modules.BatchImport;
using Infra.BatchImportAdapter.Fields;
using Infra.BatchImportAdapter.Internals;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Infra.BatchImportAdapter.Products
{
    internal class ProductModelAndParser : DataModelAndParser<ProductData>
    {
        private List<IField> fields;
        private MappingCache mappingCache;
        private readonly IEnumerable<CustomFieldDto> customFields;

        public ProductModelAndParser(IEnumerable<CustomFieldDto> customFields)
        {
            this.customFields = customFields;
        }

        public override ReadOnlyCollection<string> GetSampleRecord()
        {
            return ListFields().Select(x => Localization.Get(x.Sample)).ToList().AsReadOnly();
        }

        public override ReadOnlyCollection<IField> ListFields()
        {
            if (fields == null)
            {
                var metadata = ProductMetadata.Create();

                fields = new List<IField>(metadata.Fields.Count);

                foreach (var field in metadata.Fields)
                    fields.Add(DomainFieldAdapter.Convert(field));
            }

            return fields.AsReadOnly();
        }

        public override ProductData ParseRecord(IEnumerable<IField> fields, IDataRecord record)
        {
            if (mappingCache == null)
                mappingCache = MappingCache.Build<ProductData>(fields.ToList(), customFields.ToList());

            var product = new ProductData();

            foreach (var item in mappingCache.PropertyMappings)
            {
                var value = record[item.FieldIndex];
                var parsed = item.Field.Parse(value);
                item.Property.SetValue(product, parsed, null);
            }

            if (mappingCache.CustomFieldMappings.Any())
            {
                var customFields = new List<CustomFieldValueDto>(mappingCache.CustomFieldMappings.Count);

                foreach (var item in mappingCache.CustomFieldMappings)
                {
                    var value = record[item.FieldIndex];
                    var parsed = item.Field.Parse(value);
                    customFields[item.CustomFieldIndex] = new CustomFieldValueDto(item.CustomField.Id, parsed);
                }

                //product.CustomFields = customFields.AsReadOnly();
            }

            return product;
        }
    }
}
