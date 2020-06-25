using Application.Config;
using Application.Modules.Products;
using Domain.Entities.StoreModule;
using Domain.Modules.BatchImport;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Application.Modules.BatchImport.Products
{
    public static class ProductModelDataExtensions
    {
        public static ProductEditorViewModel AsProductViewModel(this ProductData product)
        {
            var placeholderImage = new List<ProductImage>
            {
                new ProductImage
                {
                    Position = 0,
                    Url = $"{MaaSConfig.Current.Environment.AppUrl}assets/images/product-img-placeholder.jpg"
                }
            };

            var viewModel = new ProductEditorViewModel
            {
                CustomId = product.CustomId,
                Title = product.Title,
                Description = product.Description,
                Specifications = product.Specification,
                Price = product.Price,
                CurrentStock = product.CurrentStock,
                DeleteWhenStockZero = product.DeleteWhenStockZero,
                MaxInstallments = product.MaxInstallments,
                PrivateFleetEnabled = product.Shipping_SupportsPrivateShipping,
                SedexEnabled = product.Shipping_SupportsSedexShipping,
                PACEnabled = product.Shipping_SupportsPacShipping,
                OnSitePickupEnabled = product.Shipping_SupportsOnSitePickup,
                ActualNoShippingEnabled = product.Shipping_NoShipping,
                NoShippingEnabled = product.Shipping_SupportsDownload,
                NoShippingUrl = product.Shipping_DownloadUrl,
                Height = product.Shipping_PackageHeight,
                Length = product.Shipping_PackageLength,
                Weight = product.Shipping_PackageWeight,
                Width = product.Shipping_PackageWidth,
                Category = product.CategoryId,
                SellerId = product.SellerId,
                Images = placeholderImage
            };

            viewModel.PrivateFleetParams.Add(new PrivateFleetParameterViewModel()
            {
                Range = product.Shipping_PrivateShipping_Range,
                Shipping = product.Shipping_PrivateShipping_Price
            });

            return viewModel;
        }

        internal static InvalidImportProduct ToInvalidImportProduct(this ProductData product, ErrorDictionary errors)
        {
            var metadata = ProductMetadata.Create();
            var fields = new List<InvalidImportField>(metadata.Fields.Count);

            foreach (var field in metadata.Fields)
            {
                var hasError = errors.Any(entry => entry.Key == field.Name);
                var error = errors.FirstOrDefault(entry => entry.Key == field.Name);
                var value = ValueAccessor.GetValue(product, field.Name);

                fields.Add(hasError
                    ? InvalidImportField.InvalidField(field.Name, value, error.Value)
                    : InvalidImportField.ValidField(field.Name, value));
            }

            return new InvalidImportProduct(fields);
        }

        private static class ValueAccessor
        {
            private static PropertyInfo[] properties;

            public static object GetValue(ProductData product, string propertyName)
            {
                if (properties == null)
                    BuildCache();

                return properties.Single(p => p.Name == propertyName).GetValue(product, null);
            }

            private static void BuildCache()
            {
                properties = typeof(ProductData)
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead)
                    .ToArray();
            }
        }
    }
}
