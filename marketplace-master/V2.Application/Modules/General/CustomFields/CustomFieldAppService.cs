using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2.Application.Modules.General.CustomFields
{
    public class CustomFieldAppService : ICustomFieldAppService
    {
        private readonly ICustomFieldRepository _customFieldRepository;
        private readonly ICustomFieldQueries _customFieldQueries;

        public CustomFieldAppService(
            ICustomFieldRepository customFieldRepository,
            ICustomFieldQueries customFieldQueries)
        {
            _customFieldRepository = customFieldRepository;
            _customFieldQueries = customFieldQueries;
        }

        public void ReadCustomFieldsToViewModel(IWithCustomFields entity)
        {
            entity.CustomFieldDefinitions = _customFieldRepository.DictionaryByEntityType(entity.EntityType);

            var result = _customFieldQueries.ReadValues(entity.EntityType, entity.Id);
            entity.CustomFields = result;

            if (entity.CustomFieldDefinitions.Any(x => !result.ContainsKey(x.Key.ToString())))
                AddEmptyCustomFields(entity);
        }

        public void AddEmptyCustomFields(IWithCustomFields entity)
        {
            var dict = entity.CustomFields ?? new Dictionary<string, string>();
            entity.CustomFieldDefinitions = _customFieldRepository.DictionaryByEntityType(entity.EntityType);

            foreach (var field in _customFieldRepository.ListByEntityType(entity.EntityType))
                if (entity.CustomFields == null || !entity.CustomFields.ContainsKey(field.Id.ToString()))
                    dict.Add(field.Id.ToString(), "");

            entity.CustomFields = dict;
        }

        public void StoreCustomFields(IWithCustomFields entity)
        {
            if (entity.Id == Guid.Empty)
                throw new InvalidOperationException("Cannot store custom fields for an entity with no ID.");

            var result = _customFieldRepository.SetValues(entity.Id, entity.CustomFields);

            if (result.IsFailure)
                throw new Exception($"The custom field values could not be saved for entity id {entity.Id}. Cause: " + result.Errors.ToSimpleString());
        }

        public void StoreCustomFieldsForQuoteGroup(Guid groupId, Dictionary<string, string> fieldIdsAndValues)
        {
            if (groupId == Guid.Empty)
                throw new ArgumentException("The group id must be specified.");

            if (fieldIdsAndValues == null || fieldIdsAndValues.Count == 0)
                throw new ArgumentException("The fields and values must be specified.");

            var result = _customFieldRepository.SetValuesForAllQuotesInGroup(groupId, fieldIdsAndValues);

            if (result.IsFailure)
                throw new Exception("The custom field values could not be saved for quote group id " + groupId);
        }

        public void LoadFieldDefinitionsFor(IWithCustomFields entity)
        {
            entity.CustomFieldDefinitions = _customFieldRepository.DictionaryByEntityType(entity.EntityType);
        }

        public OperationResult ValidateModel(IWithCustomFields entity)
        {
            foreach (var field in entity.CustomFieldDefinitions)
            {

                if (field.Value.IsMandatory)
                {
                    if (!entity.CustomFields.ContainsKey(field.Key.ToString()) || string.IsNullOrWhiteSpace(entity.CustomFields[field.Key.ToString()]) || entity.CustomFields[field.Key.ToString()].Equals("false"))
                    {
                        var errorMessage = GlobalizationHelper.FormatString("MANDATORY_CUSTOM_FIELD_NOT_PROVIDED", field.Value.Name);
                        return OperationResult.Fail(field.Key.ToString(), errorMessage);
                    }
                }
                else if (field.Value.InputType == CustomFieldInputType.Checkbox)
                {
                    if (!entity.CustomFields.ContainsKey(field.Key.ToString()))
                        entity.CustomFields.Add(field.Key.ToString(), "false");
                }
            }

            return OperationResult.Success;
        }


    }
}