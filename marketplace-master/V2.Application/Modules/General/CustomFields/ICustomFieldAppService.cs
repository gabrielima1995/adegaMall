using Domain.Entities;
using Framework;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.General.CustomFields
{
    public interface ICustomFieldAppService
    {
        void ReadCustomFieldsToViewModel(IWithCustomFields entity);

        void AddEmptyCustomFields(IWithCustomFields entity);

        void StoreCustomFields(IWithCustomFields entity);

        void StoreCustomFieldsForQuoteGroup(Guid groupId, Dictionary<string, string> fieldIdsAndValues);

        void LoadFieldDefinitionsFor(IWithCustomFields entity);

        OperationResult ValidateModel(IWithCustomFields entity);
    }
}
