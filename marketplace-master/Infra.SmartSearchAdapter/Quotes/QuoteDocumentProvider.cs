using Dapper;
using SmartSearch;
using SmartSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using static Dapper.SqlMapper;

namespace Infra.SmartSearchAdapter.Quotes
{
    public class QuoteDocumentProvider : IDocumentProvider
    {
        private bool disposed;
        private readonly Guid? quoteId;
        private IDocumentReader documentReader;

        public QuoteDocumentProvider()
        {
            quoteId = null;
        }

        public QuoteDocumentProvider(Guid quoteId)
        {
            this.quoteId = quoteId;
        }

        public IDocumentReader GetDocumentReader()
        {
            if (documentReader == null)
            {
                using (var dbCon = DatabaseHelper.CreateConnection())
                {
                    var script = Resources.QueryQuotesForIndexing;
                    var read = dbCon.QueryMultiple(script, new { groupId = quoteId });
                    documentReader = new QuoteDocumentReader(read);
                }
            }

            return documentReader;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                if (documentReader != null)
                    documentReader.Dispose();

                disposed = true;
            }
        }
    }

    public class QuoteDocumentReader : IDocumentReader
    {
        private readonly IEnumerable<dynamic> data;
        private int index = 0;

        internal QuoteDocumentReader(GridReader data)
        {
            this.data = data.Read();
            data.Dispose();
        }

        public IDocumentOperation CurrentDocument { get; private set; }

        public bool ReadNext()
        {
            dynamic doc;

            if (index >= data.Count())
            {
                CurrentDocument = null;
                return false;
            }
            else
            {
                doc = data.ElementAt(index);
                index++;
            }

            var operationType = ((string)doc.operation).Equals("add", StringComparison.OrdinalIgnoreCase)
                ? DocumentOperationType.AddOrUpdate
                : DocumentOperationType.Delete;

            if (operationType == DocumentOperationType.Delete)
                CurrentDocument = DocumentOperation.Delete(doc.GroupId.ToString());

            else
            {
                var fields = new Dictionary<string, object>
                {
                    { "Id", doc.Id.ToString() },
                    { "Title", (string)doc.Title },
                    { "Description", (string)doc.Description },
                    { "Phone", (string)doc.Phone },
                    { "Date", (DateTime?)doc.Date },
                    { "Preference", (int?)doc.Preference },
                    { "Amount", (decimal?)doc.Amount },
                    { "Category", (int?)doc.Category_Id },
                    { "SubCategory", (int?)doc.SubCategory_Id },
                    { "CreatedAt", (DateTime)doc.CreatedAt },
                    { "GroupId", doc.GroupId.ToString() },
                    { "BuyerName", (string)doc.FullName },
                    { "BuyerEmail", (string)doc.Email },
                    { "BuyerId", doc.Buyer_Id.ToString() },
                    { "CategoryName", (string)doc.CategoryName },
                    { "SubCategoryName", (string)doc.SubCategoryName },
                    { "PortfolioItem", (Guid?)doc.PortfolioItem_Id },
                    { "PortfolioItemTitle", (string)doc.PortfolioItemTitle },
                    { "Quantity", (int)doc.Quantity },
                    { "CityAndState", (string)doc.CityAndState }
                };

                CurrentDocument = DocumentOperation.AddOrUpdate(doc.GroupId.ToString(), fields);
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}