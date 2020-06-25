using Application.Config;
using Application.Helpers;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain.Modules.StoreModule.Price;

namespace Domain.DTO
{
    public class EcommerceHomeProducts
    {
        public Dictionary<EcommerceHomeCategory, List<EcommerceHomeItemViewModel>> Items { get; private set; }

        public EcommerceHomeProducts()
        {
            Items = new Dictionary<EcommerceHomeCategory, List<EcommerceHomeItemViewModel>>();
        }

        public void Add(EcommerceHomeCategory homeCategory, List<EcommerceHomeItemViewModel> items)
        {
            Items.Add(homeCategory, items ?? new List<EcommerceHomeItemViewModel>(0));
        }

        public EcommerceHomeCategory[] GetCategories()
        {
            return Items.Keys.ToArray();
        }

        public EcommerceHomeItemViewModel[] FilterByCategory(EcommerceHomeCategory homeCategory)
        {
            if (Items.ContainsKey(homeCategory))
                return Items[homeCategory].ToArray();

            return new EcommerceHomeItemViewModel[0];
        }
    }

    public class EcommerceHomeItemViewModel : IWishlistItem
    {
        public Guid Id { get; }
        public string LinkUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Seller { get; set; }
        public string Price { get; set; }
        public ProductPrice ProductPrice { get; set; }
        public string MultiplierPrice { get; set; }
        public Product Product { get; set; }

        public bool IsProduct { get; set; }
        public bool ShouldDisplay { get; set; }

        public EcommerceHomeItemViewModel()
        {
        }

        public EcommerceHomeItemViewModel(CustomItem customItem)
        {
            Id = customItem.Id;
            LinkUrl = customItem.TargetUrl;
            ImageUrl = customItem.ImageUrl;
            Title = customItem.Title;
            Seller = null;
            Price = null;
            IsProduct = false;

            ShouldDisplay = true;
            ShouldDisplay &= customItem.EndDate == null || DateTime.Now < customItem.EndDate;
            ShouldDisplay &= customItem.StartDate == null || DateTime.Now > customItem.StartDate;
        }

        public EcommerceHomeItemViewModel(Product product)
        {
            Id = product.Id;
            LinkUrl = MaaSConfig.Current.Environment.AppUrl + "product/" + product.FriendlyUrl;
            ImageUrl = product.CoverImageUrl;
            Title = product.Title;
            Seller = product.Store.Seller.AlternativeName ?? product.Store.Seller.Name;
            Price = product.Price.HasValue ? product.ProductPriceMapped.DisplayPrice.Value.DisplayDecimal(2): null;
            ProductPrice = product.ProductPriceMapped;
            Product = product;
            MultiplierPrice = product.MultiplierPrice?.ToString("0.00");
            IsProduct = true;
            ShouldDisplay = true;
        }

        public string GetScaledImageUrl()
        {
            try
            {
                foreach (var type in new[] { "productimage", "images", "producttemplateimage" })
                {
                    if (ImageUrl.Contains($"/{type}/") && !ImageUrl.Contains("product-img-placeholder.jpg"))
                    {
                        var id = ImageUrl.Split(new string[] { $"/{type}/" }, StringSplitOptions.None)[1];
                        var size = 330;
                        return ImgHelper.Avatar(MaaSConfig.Current.Environment.AppUrl + $"cloudimage/{type}/" + id, size);
                    }
                    if(ImageUrl.Contains("product-img-placeholder.jpg"))
                        return "/assets/images/product-img-placeholder.jpg";
                }
            }
            catch { }

            return "/assets/images/product-img-placeholder.jpg";
        }
    }
}