using Application.Config;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Interfaces;
using System;
using System.Collections.Generic;

namespace Domain.Modules.StoreModule.Price
{
    public class ProductPrice
    {
        public const string CurrentVersion = "4";

        public string Version { get; set; }

        /// <summary>
        /// Preço da unidade do produto.
        /// No caso de produto de locação, é o preço de uma unidade de locação.
        /// Caso contrário, é o próprio preço do produto.
        /// </summary>
        public Price Price { get; set; }

        /// <summary>
        /// Preço base do produto.
        /// No caso de produto de locação, com duração fixa, é o preço total da duração.
        /// Caso contrário, é o preço da unidade do produto.
        /// </summary>
        public BasePrice BasePrice { get; set; }

        /// <summary>
        /// Preço do produto para exibição.
        /// É o preço que será exibido na página de detalhes do produto.
        /// </summary>
        public DisplayPrice DisplayPrice { get; set; }

        /// <summary>
        /// Preço do produção para exibição em listas.
        /// É o preço do produto para exibir na busca e nos relacionados, com valores diferentes a serem exibidos da página de detalhes.
        /// </summary>
        public DisplayPrice DisplayShortPrice { get; set; }

        public bool HasBasePrice =>
            MaaSConfig.Current.Product.Price.Enabled && DisplayPrice.HasPrice;

        private static Price GetShortPrice(Product product, decimal price, decimal? originalPrice)
        {
            if (product.IsPackageType && product.PackageCredits.HasValue)
            {
                price = Math.Round(price / product.PackageCredits.Value, 5);

                if (originalPrice.HasValue)
                {
                    originalPrice = Math.Round(originalPrice.Value / product.PackageCredits.Value, 5);
                }
            }

            return Price.New(price, originalPrice);
        }

        private static BasePrice GetShortBasePrice(Product product, decimal price, decimal? originalPrice)
        {
            if (product.IsPackageType && product.PackageCredits.HasValue)
            {
                price = Math.Round(price / product.PackageCredits.Value, 5);

                if (originalPrice.HasValue)
                {
                    originalPrice = Math.Round(originalPrice.Value / product.PackageCredits.Value, 5);
                }
            }

            return BasePrice.New(price, originalPrice);
        }

        public static ProductPrice ByPrice(Product product, decimal price, decimal? originalPrice, IList<RentalShiftDto> shifts)
        {
            var pp = new ProductPrice
            {
                Version = CurrentVersion,
                Price = Price.New(price, originalPrice)
            };

            pp.BasePrice = BasePrice.Calculate(product, pp.Price);
            pp.DisplayPrice = DisplayPrice.New(product, pp.Price, false, shifts);

            var shortPrice = GetShortPrice(product, price, originalPrice);
            pp.DisplayShortPrice = DisplayPrice.New(product, shortPrice, true, shifts);

            return pp;
        }

        public static ProductPrice ByBasePrice(Product product, decimal price, decimal? originalPrice, IList<RentalShiftDto> shifts)
        {
            var pp = new ProductPrice
            {
                Version = CurrentVersion,
                BasePrice = BasePrice.New(price, originalPrice)
            };

            pp.Price = Price.Calculate(product, pp.BasePrice);
            pp.DisplayPrice = DisplayPrice.New(product, pp.Price, false, shifts);

            var shortBasePrice = GetShortBasePrice(product, price, originalPrice);
            var shortPrice = Price.Calculate(product, shortBasePrice);
            pp.DisplayShortPrice = DisplayPrice.New(product, shortPrice, true, shifts);

            return pp;
        }
    }
}