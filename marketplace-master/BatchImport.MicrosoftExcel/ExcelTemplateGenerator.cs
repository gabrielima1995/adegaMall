using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BatchImport.MicrosoftExcel
{
    public class ExcelTemplateGenerator : ITemplateGenerator
    {
        public void WriteTemplate(IDataModel dataModel, Stream output)
        {
            var fields = dataModel.ListFields();

            using (var book = new XLWorkbook())
            {
                var sheet = book.Worksheets.Add("1");

                AddHeader(sheet, fields);
                AddSampleRecord(sheet, dataModel);

                book.SaveAs(output);
            }
        }

        public Task WriteTemplateAsync(IDataModel dataModel, Stream output)
        {
            WriteTemplate(dataModel, output);
            return Task.CompletedTask;
        }

        private void AddHeader(IXLWorksheet sheet, IEnumerable<IField> fields)
        {
            var column = 1;

            foreach (var f in fields)
            {
                var cell = sheet.Cell(1, column++);
                cell.Value = f.DisplayName;
                cell.Style.Fill.BackgroundColor = XLColor.Black;
                cell.Style.Font.FontColor = XLColor.White;
            }
        }

        private void AddSampleRecord(IXLWorksheet sheet, IDataModel dataModel)
        {
            var record = dataModel.GetSampleRecord();
            var column = 1;

            foreach (var value in record)
                sheet.Cell(2, column++).Value = value;
        }
    }
}
