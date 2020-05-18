using Application.Config;
using Domain.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Models
{
    public class EcommerceHomeProductsVM
    {
        private readonly EcommerceHomeProducts products;
        private List<EcommerceHomeCategory> categories;

        public EcommerceHomeProductsVM()
        {
        }

        public EcommerceHomeProductsVM(EcommerceHomeProducts ecommerceHomeProductsDto)
        {
            products = ecommerceHomeProductsDto;
        }

        public List<EcommerceHomeCategory> Categories
        {
            get
            {
                if (categories == null)
                {
                    categories = new List<EcommerceHomeCategory>();

                    if (products != null)
                    {
                        foreach (var item in products.Items)
                        {
                            var hasProducts = item.Value.Any();
                            if (hasProducts) categories.Add(item.Key);
                        }
                    }
                }

                return categories;
            }
        }

        public List<EcommerceHomeItemViewModel> GetProducts(EcommerceHomeCategory category)
        {
            if (products == null) return new List<EcommerceHomeItemViewModel>();

            var listProducts = products.FilterByCategory(category).ToList();
            if (MaaSConfig.Current.EcommerceProducts.EnableRandomOrder)
            {
                Random random = new Random(DateTime.Now.Millisecond);
                listProducts = listProducts.OrderBy(x => random.NextDouble()).ToList();
            }

            return listProducts;
        }
    }
}