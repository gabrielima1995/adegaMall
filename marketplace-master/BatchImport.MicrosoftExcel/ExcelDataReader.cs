using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BatchImport.MicrosoftExcel
{
    public class ExcelDataReader : IDataReader
    {
        private bool disposed = false;
        private int row = 0;
        private int rowsCount;
        private Dictionary<string, int> nameToIndex;
        private Stream stream;
        private XLWorkbook book;
        private IXLWorksheet sheet;

        public string this[int i] => GetCellValue(i + 1);

        public string this[string name] => this[nameToIndex[name]];

        public int FieldCount => nameToIndex.Count;

        public ExcelDataReader() { }

        public ExcelDataReader(Stream stream)
        {
            Load(stream);
        }

        public void Load(Stream stream)
        {
            this.stream = stream;
        }

        public bool ReadNext()
        {
            if (book == null)
                Initilize();

            return ++row <= rowsCount;
        }

        public Task<bool> ReadNextAsync()
        {
            return Task.FromResult(ReadNext());
        }

        private string GetCellValue(int colNumber)
        {
            var str = sheet.Row(row).Cell(colNumber).GetString()?.Trim();
            return string.IsNullOrEmpty(str) ? null : str;
        }

        private void Initilize()
        {
            book = new XLWorkbook(stream);

            if (book.Worksheets.Count == 0)
                throw new SpreadsheetHasNoPagesException();

            sheet = book.Worksheets.First();
            rowsCount = sheet.RowsUsed().Count();

            if (rowsCount == 0)
                throw new SpreadsheetHasNoRows();

            row = 1;
            ReadHeader();
        }

        private void ReadHeader()
        {
            var cols = sheet.Row(row).CellsUsed().Count();

            if (cols == 0)
                throw new InvalidOperationException("The header must not be empty.");

            nameToIndex = new Dictionary<string, int>(cols);

            for (int i = 0; i < cols; i++)
            {
                var name = this[i];

                if (name == null)
                    break;

                nameToIndex.Add(name, i);
            }
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (nameToIndex != null)
                {
                    nameToIndex.Clear();
                    nameToIndex = null;
                }

                if (sheet != null)
                    sheet.Dispose();

                if (book != null)
                    book.Dispose();

                if (stream != null)
                    stream.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
