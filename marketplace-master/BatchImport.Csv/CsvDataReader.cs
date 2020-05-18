using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BatchImport.Csv
{
    public class CsvDataReader : IDataReader
    {
        private bool disposed = false;
        private Dictionary<string, int> nameToIndex;
        private Stream stream;
        private StreamReader streamReader;
        private CsvReader csvReader;

        public CsvDataReader()
        {
        }

        public CsvDataReader(Stream stream)
        {
            Load(stream);
        }

        public int FieldCount => csvReader.Context.HeaderRecord.Length;

        public string this[string name] => this[nameToIndex[name]];

        public string this[int i] => GetValue(i);

        public void Load(Stream stream)
        {
            this.stream = stream;
        }

        public bool ReadNext()
        {
            if (csvReader == null)
                InitializeCsvReader();

            return csvReader.Read();
        }

        public async Task<bool> ReadNextAsync()
        {
            if (csvReader == null)
                InitializeCsvReader();

            return await csvReader.ReadAsync();
        }

        private string GetValue(int index)
        {
            var str = csvReader[index]?.Trim();
            return string.IsNullOrEmpty(str) ? null : str;
        }

        private void InitializeCsvReader()
        {
            streamReader = new StreamReader(stream);
            csvReader = new CsvReader(streamReader);

            if (csvReader.Read())
            {
                if (csvReader.ReadHeader())
                {
                    var len = csvReader.Context.HeaderRecord.Length;
                    nameToIndex = new Dictionary<string, int>(len);

                    for (int i = 0; i < len; i++)
                        nameToIndex.Add(csvReader.Context.HeaderRecord[i], i);
                }
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
                nameToIndex.Clear();
                nameToIndex = null;

                if (csvReader != null)
                    csvReader.Dispose();

                if (streamReader != null)
                    streamReader.Dispose();

                if (stream != null)
                    stream.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
