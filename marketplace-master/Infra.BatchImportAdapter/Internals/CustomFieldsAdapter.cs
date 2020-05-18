using Application.Modules.BatchImport.Models;
using BatchImport;
using BatchImport.Fields;
using Dapper;
using Domain.Entities;
using Framework;
using Infra.BatchImportAdapter.Fields;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.BatchImportAdapter.Internals
{
    /// <summary>
    /// Provides helpers for working with domain CustomFields (retrieving and converting them into IField instances).
    /// </summary>
    public class CustomFieldsAdapter
    {
        private readonly ConnectionString connectionString;

        public CustomFieldsAdapter(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<CustomFieldDto>> GetCustomFieldsAsync(CustomFieldEntityType entityType)
        {
            var lang = GetCurrentLang();

            using (var conn = GetDbConnection())
            {
                var queries = await conn.QueryMultipleAsync(
                    Resources.BatchImportQueries_GetCustomFields, new { entityType, lang });

                var fields = await queries.ReadAsync<CustomFieldDto>();
                var options = await queries.ReadAsync<CustomFieldOptionDtoEx>();

                if (options.Any())
                    foreach (var f in fields)
                        f.Options = options.Where(o => o.FieldId == f.Id);

                return fields;
            }
        }

        public static IEnumerable<IField> Convert(IEnumerable<CustomFieldDto> customFields)
        {
            return customFields.Select(f => Convert(f));
        }

        private static IField Convert(CustomFieldDto customField)
        {
            switch (customField.DataType)
            {
                case CustomFieldDataType.RawText:
                    return new StringField(customField.Id.ToString(), customField.Name, customField.Placeholder, customField.IsMandatory);

                case CustomFieldDataType.Number:
                    return new DecimalField(customField.Id.ToString(), customField.Name, customField.Placeholder, customField.IsMandatory);

                case CustomFieldDataType.Date:
                    return new DateField(customField.Id.ToString(), customField.Name, customField.Placeholder, customField.IsMandatory);

                case CustomFieldDataType.DateTime:
                    return new DateField(customField.Id.ToString(), customField.Name, customField.Placeholder, customField.IsMandatory);

                case CustomFieldDataType.Boolean:
                    return new YesNoField(customField.Id.ToString(), customField.Name, customField.Placeholder, customField.IsMandatory);

                default:
                    throw new UnknownCustomFieldDataTypeException(customField.DataType);
            }
        }

        private string GetCurrentLang()
        {
            var threadLang = Thread.CurrentThread.GetCurrentLang();
            return threadLang.IndexOf("en", StringComparison.OrdinalIgnoreCase) >= 0 ? "EN" : null;
        }

        private DbConnection GetDbConnection()
        {
            return new SqlConnection(connectionString.Value);
        }

        internal class CustomFieldOptionDtoEx : CustomFieldOptionDto
        {
            public Guid FieldId { get; set; }
        }

        public class UnknownCustomFieldDataTypeException : Exception
        {
            public UnknownCustomFieldDataTypeException(CustomFieldDataType dataType)
                : base($"O tipo de campo customizado '{dataType}' não é suportado.") { }
        }
    }
}
