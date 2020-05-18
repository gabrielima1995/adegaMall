using Application.Modules.BatchImport.Models;
using BatchImport;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Infra.BatchImportAdapter.Internals
{
    /// <summary>
    /// Holds an index cache for performance when mapping an IRecord to model class.
    /// </summary>
    internal class MappingCache
    {
        internal ReadOnlyCollection<PropertyMappingCacheItem> PropertyMappings { get; }
        internal ReadOnlyCollection<CustomFieldMappingCacheItem> CustomFieldMappings { get; }

        internal MappingCache(IEnumerable<PropertyMappingCacheItem> propertyMappings, IEnumerable<CustomFieldMappingCacheItem> customFieldMappings)
        {
            PropertyMappings = propertyMappings.ToList().AsReadOnly();
            CustomFieldMappings = customFieldMappings.ToList().AsReadOnly();
        }

        internal static MappingCache Build<T>(IList<IField> fields, IList<CustomFieldDto> customFieldDefinitions)
        {
            var propertyItems = BuildPropertyCacheItems<T>(fields);
            var customFieldItems = BuildCustomFieldCacheItems(fields, customFieldDefinitions);
            return new MappingCache(propertyItems, customFieldItems);
        }

        private static List<PropertyMappingCacheItem> BuildPropertyCacheItems<T>(IList<IField> fields)
        {
            var properties = typeof(T).GetProperties().Where(p => p.CanWrite).ToArray();
            var items = new List<PropertyMappingCacheItem>(properties.Length);

            for (int i = 0; i < fields.Count; i++)
            {
                var matchingProperty = properties.SingleOrDefault(p => p.Name == fields[i].Name);
                if (matchingProperty == null)
                    continue;

                items.Add(new PropertyMappingCacheItem(fields[i], i, matchingProperty));
            }

            return items;
        }

        private static ReadOnlyCollection<CustomFieldMappingCacheItem> BuildCustomFieldCacheItems(IList<IField> fields, IList<CustomFieldDto> customFieldDefinitions)
        {
            var items = new List<CustomFieldMappingCacheItem>(customFieldDefinitions.Count);

            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] is CustomFieldWrapper)
                {
                    for (int j = 0; j < customFieldDefinitions.Count; j++)
                    {
                        if (customFieldDefinitions[j].Id.ToString() != fields[i].Name)
                            continue;

                        items.Add(new CustomFieldMappingCacheItem(fields[i], i, customFieldDefinitions[j], j));
                        break;
                    }
                }
            }

            return items.AsReadOnly();
        }

        #region Nested types

        internal abstract class MappingCacheItem
        {
            internal MappingCacheItem(IField field, int fieldIndex)
            {
                Field = field;
                FieldIndex = fieldIndex;
            }

            internal IField Field { get; }
            internal int FieldIndex { get; }
        }

        internal class PropertyMappingCacheItem : MappingCacheItem
        {
            internal PropertyMappingCacheItem(IField field, int fieldIndex, PropertyInfo property) : base(field, fieldIndex)
            {
                Property = property;
            }

            internal PropertyInfo Property { get; }
        }

        internal class CustomFieldMappingCacheItem : MappingCacheItem
        {
            internal CustomFieldMappingCacheItem(IField field, int fieldIndex, CustomFieldDto customField, int customFieldIndex) : base(field, fieldIndex)
            {
                CustomField = customField;
                CustomFieldIndex = customFieldIndex;
            }

            internal CustomFieldDto CustomField { get; }
            internal int CustomFieldIndex { get; }
        }

        #endregion
    }
}
