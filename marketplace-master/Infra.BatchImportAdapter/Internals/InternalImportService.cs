using Application.Modules.BatchImport.Models;
using BatchImport;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.BatchImportAdapter.Internals
{
    internal static class InternalImportService
    {
        public static async Task GenerateTemplateAsync(BatchImportSourceType sourceType, IDataModel dataModel, Stream output)
        {
            var generator = TemplateGeneratorFactory.Create(sourceType);
            await generator.WriteTemplateAsync(dataModel, output);
        }

        public static async Task<ImportResult<T>> ImportAsync<T>(DataModelAndParser<T> model, BatchImportSourceType sourceType, Stream input)
        {
            var fields = model.ListFields();

            var validRecords = new List<T>();
            var invalidRecords = new List<IInvalidRecord>();

            using (var reader = DataReaderFactory.Create(sourceType, input))
            {
                while (await reader.ReadNextAsync())
                {
                    var validation = model.ValidateRecord(reader);

                    if (!validation.IsValid)
                    {
                        invalidRecords.Add(InvalidRecord.Create(validation));
                        continue;
                    }

                    var parsed = model.ParseRecord(fields, reader);
                    validRecords.Add(parsed);
                }
            }

            return new ImportResult<T>(
                fields.Select(f => f.DisplayName),
                validRecords, invalidRecords);
        }
    }
}
