using Application.Config;
using Dapper;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule;
using SmartSearch;
using SmartSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Dapper.SqlMapper;

namespace Infra.SmartSearchAdapter.Products
{
    public class ProductDocumentProvider : IDocumentProvider
    {
        private bool disposed;
        private readonly RentalSearchDocumentService rentalSearchDocumentService;
        private readonly Guid? productId;
        private readonly string sellerId;
        private IDocumentReader documentReader;

        public ProductDocumentProvider(RentalSearchDocumentService rentalSearchDocumentService)
        {
            this.rentalSearchDocumentService = rentalSearchDocumentService;
        }

        public ProductDocumentProvider(RentalSearchDocumentService rentalSearchDocumentService, Guid productId)
            : this(rentalSearchDocumentService)
        {
            this.productId = productId;
        }

        public ProductDocumentProvider(RentalSearchDocumentService rentalSearchDocumentService, string sellerId)
            : this(rentalSearchDocumentService)
        {
            this.sellerId = sellerId;
        }

        public IDocumentReader GetDocumentReader()
        {
            if (documentReader == null)
            {
                using (var dbCon = DatabaseHelper.CreateConnection())
                {
                    var script = Resources.QueryProductsForIndexing;
                    var read = dbCon.QueryMultiple(script, new
                    {
                        productId,
                        sellerId,
                        shippingPerStore = MaaSConfig.Current.Product.ShippingMode == ShippingMode.PerStore
                    }, commandTimeout: 300);
                    documentReader = new ProductDocumentReader(rentalSearchDocumentService, read);
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

    public class ProductDocumentReader : IDocumentReader
    {
        private readonly IEnumerable<dynamic> data;
        private readonly RentalSearchDocumentService rentalSearchDocumentService;
        private int index;

        internal ProductDocumentReader(RentalSearchDocumentService rentalSearchDocumentService, GridReader data)
        {
            this.data = data.Read();
            data.Dispose();

            this.rentalSearchDocumentService = rentalSearchDocumentService;
        }

        public IDocumentOperation CurrentDocument { get; private set; }

        public bool ReadNext()
        {
            if (index >= data.Count())
            {
                CurrentDocument = null;
                return false;
            }

            var doc = data.ElementAt(index);
            index++;

            var operationType = ((string)doc.operation).Equals("add", StringComparison.OrdinalIgnoreCase)
                ? DocumentOperationType.AddOrUpdate
                : DocumentOperationType.Delete;

            if (operationType == DocumentOperationType.Delete)
                CurrentDocument = DocumentOperation.Delete(doc.Id.ToString());

            else
            {
                var fields = new Dictionary<string, object>
                {
                    { "Id", (Guid)doc.Id },
                    { "Title", (string)doc.Title },
                    { "Description", (string)doc.Description },
                    { "Specifications", (string)doc.Specifications },
                    { "CustomId", (string)doc.CustomId },
                    { "CoverImageUrl", (string)doc.CoverImageUrl },
                    { "FriendlyUrl", (string)doc.FriendlyUrl },
                    { "CreatedAt", (DateTime)doc.CreatedAt },
                    { "UpdatedAt", (DateTime?)doc.UpdatedAt },
                    { "ProductPrice", (string)doc.ProductPrice },
                    { "BasePrice", (decimal?)doc.BasePrice },

                    { "HasStock", (bool)doc.HasStock },
                    { "IsDisabled", (bool)doc.IsDisabled },
                    { "HasTemplate", (bool)doc.HasTemplate },
                    { "IsBestPrice", (bool)doc.IsBestPrice },
                    { "TemplateId", (Guid?)doc.TemplateId },
                    { "NoShippingEnabled", (bool)doc.NoShippingEnabled },
                    { "ActualNoShippingEnabled", (bool)doc.ActualNoShippingEnabled },
                    { "OnSitePickupEnabled", (bool)doc.OnSitePickupEnabled },
                    { "SedexEnabled", (bool)doc.SedexEnabled },
                    { "PACEnabled", (bool)doc.PACEnabled },
                    { "PrivateFleetEnabled", (bool)doc.PrivateFleetEnabled },

                    { "CategoryId", (int?)doc.CategoryId },
                    { "CategoryName", (string)doc.CategoryName },
                    { "CategorySlug", (string)doc.CategorySlug },

                    { "SellerId", (string)doc.SellerId },
                    { "SellerNickname", (string)doc.SellerNickname },
                    { "SellerProfileName", (string)doc.SellerProfileName },
                    { "SellerFriendlyUrl", (string)doc.SellerFriendlyUrl },
                    { "SellerCreatedAt", (DateTime)doc.SellerCreatedAt },

                    { "ImageUrls", SplitValues((string)doc.ImageUrls) },
                    { "DeliveryRanges", SplitValues((string)doc.DeliveryRanges, true) },
                    { "VariantIds", SplitValues((string)doc.VariantIds, true) },
                    { "VariantNames", SplitValues((string)doc.VariantNames) },
                    { "VariantSlugs", SplitValues((string)doc.VariantSlugs) },

                    { "AddressStreet", (string)doc.AddressStreet },
                    { "AddressNumber", (string)doc.AddressNumber },
                    { "AddressAdditionalInfo", (string)doc.AddressAdditionalInfo },
                    { "AddressNeighborhood", (string)doc.AddressNeighborhood },
                    { "AddressCityAndState", (string)doc.AddressCityAndState },
                    { "AddressZipCode", (string)doc.AddressZipCode },
                    { "AddressLatLng", ParseLatLng((string)doc.AddressLatLng) },

                    { "HasActiveSubscription", (bool)doc.HasActiveSubscription },
                    { "HasPaymentGatewayAccount", (bool)doc.HasPaymentGatewayAccount },
                    { "HasBeenApprovedInModeration", (bool)doc.HasBeenApprovedInModeration },
                    { "AverageRating", (decimal)doc.AverageRating }
                };

                var rentalSearchDocument = rentalSearchDocumentService.GetDocument((Guid)doc.Id);

                fields.Add("AvailableOnWeekends", GetAvailableOnWeekends(rentalSearchDocument));
                fields.Add("AvailableMondayToFriday", GetAvailableMondayToFriday(rentalSearchDocument));
                fields.Add("AvailableMorning", GetAvailableMorning(rentalSearchDocument));
                fields.Add("AvailableAfternoon", GetAvailableAfternoon(rentalSearchDocument));
                fields.Add("AvailableNight", GetAvailableNight(rentalSearchDocument));

                CurrentDocument = DocumentOperation.AddOrUpdate(doc.Id.ToString(), fields);
            }

            return true;
        }

        private int[] GetAvailableOnWeekends(RentalSearchDocument document) => document.Entries.Where(entry => entry.IsWeekend).Select(entry => IntDateHelper.Convert(entry.Date)).ToArray();
        private int[] GetAvailableMondayToFriday(RentalSearchDocument document) => document.Entries.Where(entry => entry.IsMondayToFriday).Select(entry => IntDateHelper.Convert(entry.Date)).ToArray();
        private int[] GetAvailableMorning(RentalSearchDocument document) => document.Entries.Where(entry => entry.AvailableMorning).Select(entry => IntDateHelper.Convert(entry.Date)).ToArray();
        private int[] GetAvailableAfternoon(RentalSearchDocument document) => document.Entries.Where(entry => entry.AvailableAfternoon).Select(entry => IntDateHelper.Convert(entry.Date)).ToArray();
        private int[] GetAvailableNight(RentalSearchDocument document) => document.Entries.Where(entry => entry.AvailableNight).Select(entry => IntDateHelper.Convert(entry.Date)).ToArray();

        private object[] SplitValues(string input, bool convertToInt = false)
        {
            if (input == null)
                return Array.Empty<object>();

            var values = input.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            return convertToInt
                ? values.Select(long.Parse).Cast<object>().ToArray()
                : values.Cast<object>().ToArray();
        }

        private object ParseLatLng(string value)
        {
            if (value == null)
                return null;

            var parts = value.Split('|');
            return new LatLng(
                double.Parse(parts[0], CultureInfo.InvariantCulture),
                double.Parse(parts[1], CultureInfo.InvariantCulture));
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