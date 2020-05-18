using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BatchImport.Csv
{
    public class CsvTemplateGenerator : ITemplateGenerator
    {
        public void WriteTemplate(IDataModel dataModel, Stream output)
        {
            var writer = new StreamWriter(output);
            var csv = new CsvWriter(writer);
            var fields = dataModel.ListFields();

            AddHeader(csv, fields);
            csv.NextRecord();

            AddSampleRecord(csv, dataModel);
            csv.NextRecord();

            writer.Flush();
        }

        public async Task WriteTemplateAsync(IDataModel dataModel, Stream output)
        {
            var writer = new StreamWriter(output);
            var csv = new CsvWriter(writer);
            var fields = dataModel.ListFields();

            AddHeader(csv, fields);
            await csv.NextRecordAsync();

            AddSampleRecord(csv, dataModel);
            await csv.NextRecordAsync();

            await writer.FlushAsync();
        }

        private void AddHeader(CsvWriter csv, IEnumerable<IField> fields)
        {
            foreach (var f in fields)
                csv.WriteField(f.DisplayName);
        }

        private void AddSampleRecord(CsvWriter csv, IDataModel dataModel)
        {
            var record = dataModel.GetSampleRecord();

            foreach (var value in record)
                csv.WriteField(value);
        }
    }
}
