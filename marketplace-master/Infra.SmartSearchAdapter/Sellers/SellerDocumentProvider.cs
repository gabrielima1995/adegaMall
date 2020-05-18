using Application.Config;
using Domain.Modules.StoreModule.CompleteProfile;
using Domain.Specifications;
using Infra.SmartSearchAdapter.Common;
using Newtonsoft.Json;
using SmartSearch;
using SmartSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Infra.SmartSearchAdapter.Sellers
{
    public sealed class SellerDocumentProvider : IDocumentProvider
    {
        private bool disposed;
        private IDocumentReader documentReader;

        public string SellerId { get; set; }

        public SellerDocumentProvider()
        {
        }

        public SellerDocumentProvider(string sellerId)
        {
            SellerId = sellerId;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (documentReader != null)
                    documentReader.Dispose();
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        public IDocumentReader GetDocumentReader()
        {
            if (documentReader == null)
                documentReader = new SellerDocumentReader(new SellerSearchDomain(), SellerId);

            return documentReader;
        }
    }

    public sealed class SellerDocumentReader : SqlDocumentReader
    {
        private readonly SellerShouldAppearInSearchSpecification specification;
        private Dictionary<string, int> columnIndexCache;

        public string SellerId { get; }

        public SellerDocumentReader(ISearchDomain searchDomain, string sellerId) : base(searchDomain)
        {
            specification = new SellerShouldAppearInSearchSpecification();
            SellerId = sellerId;
        }

        protected override SqlCommand GetCommand()
        {
            var cmd = SqlConnection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Resources.QuerySellersForIndexing;
            cmd.Parameters.Add("sellerId", SqlDbType.VarChar, 128).Value = SellerId ?? (object)DBNull.Value;
            cmd.Parameters.Add("checkHasCategories", SqlDbType.Bit).Value = MaaSConfig.Current.Quote.Enabled;
            cmd.Parameters.Add("checkHasPortfolio", SqlDbType.Bit).Value = MaaSConfig.Current.Portfolio.Enabled;
            cmd.Parameters.Add("checkHasServicesOrProducts", SqlDbType.Bit).Value = MaaSConfig.Current.Product.Enabled && !MaaSConfig.Current.Quote.QuoteBilling.Enabled;
            cmd.Parameters.Add("calculateAvarageRating", SqlDbType.Bit).Value = MaaSConfig.Current.Seller.UserRatings.Enabled;

            return cmd;
        }

        protected override SqlConnection GetConnection()
        {
            return DatabaseHelper.CreateConnection() as SqlConnection;
        }

        protected override string GetDocumentId(IDataRecord row)
        {
            return row[SellerFieldNames.Id].ToString();
        }

        protected override bool IsDeleteOperation(IDataRecord row)
        {
            if (columnIndexCache == null)
                BuildColumnIndexCache();

            var sellerShouldAppear = specification.IsSatisfiedBy(new SellerForAppearInSearchSpecification(
                row.GetBoolean(columnIndexCache[SellerFieldNames.Enabled]),
                row.GetValue(columnIndexCache[SellerFieldNames.CompleteProfile]).ToString()));

            return !sellerShouldAppear;
        }

        protected override object PrepareFieldValue(string fieldName, object value)
        {
            if (fieldName == SellerFieldNames.CategoryIds)
                return SplitCategories(value?.ToString(), convertToInt: true);

            if (fieldName == SellerFieldNames.CategoryNames)
                return SplitCategories(value?.ToString(), convertToInt: false);

            if (fieldName == SellerFieldNames.SubCategoryIds)
                return SplitCategories(value?.ToString(), convertToInt: true);

            if (fieldName == SellerFieldNames.SubCategoryNames)
                return SplitCategories(value?.ToString(), convertToInt: false);

            if (fieldName == SellerFieldNames.AddressLatLng)
            {
                if (value != null)
                {
                    var parts = value.ToString().Split('|');
                    return new LatLng(
                        double.Parse(parts[0], CultureInfo.InvariantCulture),
                        double.Parse(parts[1], CultureInfo.InvariantCulture));
                }
            }

            if(fieldName == SellerFieldNames.CompleteProfile)
            {
                value = JsonConvert.DeserializeObject<SellerCompleteProfile>(value.ToString()).IsComplete;
            }

            return value;
        }

        private void BuildColumnIndexCache()
        {
            var fields = new[]
            {
                SellerFieldNames.Enabled,
                SellerFieldNames.CompleteProfile
            };

            columnIndexCache = new Dictionary<string, int>(fields.Length);

            foreach (var field in fields)
            {
                var index = DataReader.GetOrdinal(field);
                columnIndexCache.Add(field, index);
            }
        }

        private object[] SplitCategories(string input, bool convertToInt)
        {
            if (input == null)
                return new object[0];

            var values = input.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            return convertToInt
                ? values.Select(str => (object)int.Parse(str)).ToArray()
                : values.ToArray();
        }
    }
}