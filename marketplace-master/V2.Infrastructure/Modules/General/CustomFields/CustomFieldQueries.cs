using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using V2.Application.Modules.General.CustomFields;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.General.CustomFields
{
    public class CustomFieldQueries : ICustomFieldQueries
    {
        private readonly string sql_readScript = @"SELECT * FROM CustomFieldValue WHERE EntityId = '{0}' AND CustomField_Id IS NOT NULL;";
        private readonly string sql_readScriptCustomFieldDto = @"
                    SELECT
						CustomField_Id,
						CustomField.Name as Name, 
                        CustomFieldValue.Value as Value, 
                        CustomFieldValue.EntityId as EntityId
                    FROM CustomField
                    LEFT JOIN CustomFieldValue ON CustomField.Id = CustomFieldValue.CustomField_Id
					    AND CustomFieldValue.EntityId = '{0}' 
                        AND EntityType = '{1}'";

        private readonly ICustomFieldRepository _customFieldRepository;

        public CustomFieldQueries(ICustomFieldRepository customFieldRepository)
        {
            _customFieldRepository = customFieldRepository;
        }

        public Dictionary<string, string> ReadValues(CustomFieldEntityType entityType, Guid entityId)
        {
            using (var connection = MyDbConnection.Create())
            {
                var script = string.Format(sql_readScript, entityId.ToString());
                var queryResult = connection.Query(script);
                var result = new Dictionary<string, string>();

                foreach (var item in queryResult)
                {
                    try
                    {
                        if (item == null || item.Value == null)
                            continue;

                        if (item.CustomField_Id != null)
                            result.Add(item.CustomField_Id.ToString(), item.Value.ToString());
                    }
                    catch
                    {
                        continue;
                    }
                }

                return result;
            }
        }

        public List<CustomFieldValueDto> ReadValuesForEntityId(CustomFieldEntityType entityType, Guid entityId)
        {
            using (var connection = MyDbConnection.Create())
            {
                object[] queryParam =
                {
                    entityId.ToString(),
                    (int)entityType
                };
                var script = string.Format(sql_readScriptCustomFieldDto, queryParam);
                var queryResult = connection.Query(script);
                var customFields = new List<CustomFieldValueDto>();
                foreach (var item in queryResult)
                {
                    try
                    {
                        if (item == null || item.Value == null)
                            continue;

                        if (item.CustomField_Id != null)
                            customFields.Add(new CustomFieldValueDto(item.EntityId.ToString(), item.Value.ToString(), item.Name.ToString()));
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                return customFields;
            }
        }
    }

}