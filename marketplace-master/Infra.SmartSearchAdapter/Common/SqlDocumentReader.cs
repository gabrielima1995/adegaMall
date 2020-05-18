using Domain.Modules.StoreModule.CompleteProfile;
using Newtonsoft.Json;
using SmartSearch;
using SmartSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infra.SmartSearchAdapter.Common
{
    public abstract class SqlDocumentReader : IDocumentReader
    {
        private bool disposed;
        private bool isInitialized;
        private Dictionary<string, int> columnIndexCache;

        protected SqlConnection SqlConnection { get; private set; }
        protected SqlCommand SqlCommand { get; private set; }
        protected SqlDataReader DataReader { get; private set; }

        public ISearchDomain SearchDomain { get; }
        public IDocumentOperation CurrentDocument { get; protected set; }

        public SqlDocumentReader(ISearchDomain searchDomain)
        {
            SearchDomain = searchDomain;
        }

        public bool ReadNext()
        {
            if (!isInitialized)
                Initialize();

            if (!DataReader.Read())
            {
                CurrentDocument = null;
                return false;
            }

            ReadDataReaderIntoCurrentDocument();
            return true;
        }

        protected virtual void ReadDataReaderIntoCurrentDocument()
        {
            if (columnIndexCache == null)
                BuildColumnIndexCache();

            var id = GetDocumentId(DataReader);

            if (IsDeleteOperation(DataReader))
            {
                CurrentDocument = DocumentOperation.Delete(id);
                return;
            }

            var documentBuilder = new DocumentBuilder(id);
            foreach (var field in SearchDomain.Fields)
            {
                var columnIndex = columnIndexCache[field.Name];
                var value = DataReader.IsDBNull(columnIndex) ? null : DataReader.GetValue(columnIndex);
                value = PrepareFieldValue(field.Name, value);
                documentBuilder.Add(field.Name, value);
            }

            CurrentDocument = DocumentOperation.AddOrUpdate(documentBuilder.Build());
        }

        protected virtual object PrepareFieldValue(string fieldName, object value)
        {
            return value;
        }

        private void BuildColumnIndexCache()
        {
            columnIndexCache = new Dictionary<string, int>(SearchDomain.Fields.Count);

            foreach (var field in SearchDomain.Fields)
            {
                var index = DataReader.GetOrdinal(field.Name);
                columnIndexCache.Add(field.Name, index);
            }
        }

        private void Initialize()
        {
            SqlConnection = GetConnection();
            SqlCommand = GetCommand();

            SqlConnection.Open();
            DataReader = SqlCommand.ExecuteReader();

            isInitialized = true;
        }

        protected abstract SqlConnection GetConnection();

        protected abstract SqlCommand GetCommand();

        protected abstract string GetDocumentId(IDataRecord row);

        protected abstract bool IsDeleteOperation(IDataRecord row);

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                if (DataReader != null && !DataReader.IsClosed)
                    DataReader.Close();

                if (SqlCommand != null)
                    SqlCommand.Dispose();

                if (SqlConnection != null)
                    SqlConnection.Dispose();

                disposed = true;
            }
        }

        #endregion Dispose
    }
}