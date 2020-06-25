using Domain.Entities.StoreModule;
using Domain.StoreModule.Interfaces;
using System;
using System.Collections.Generic;

namespace Domain.Modules.StoreModule.Price
{
    public class DiscountPrice
    {
        public decimal Amount { get; set; }

        public decimal Percent { get; set; }

        public string Formatted { get; set; }

        public DisplayPrice Original { get; set; }

        public bool HasDiscount => Original != null;

        public static DiscountPrice Empty() => new DiscountPrice();

        public static DiscountPrice New(Product product, Price price, bool shortPrice, IList<RentalShiftDto> shifts)
        {
            var discountPrice = Empty();

            if (price.HasOriginal)
            {
                discountPrice.Amount = price.Original.Value - price.Value;
                discountPrice.Percent = Convert.ToInt32(Math.Floor(discountPrice.Amount / price.Original.Value * 100));
                discountPrice.Formatted = $"{discountPrice.Percent}% off";
                discountPrice.Original = DisplayPrice.New(product, price.Original, shortPrice, shifts);
            }

            return discountPrice;
        }
    }
}