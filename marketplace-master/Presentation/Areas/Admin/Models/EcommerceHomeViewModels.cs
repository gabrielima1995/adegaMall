using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Presentation.Areas.Admin.Models
{
    public class EcommerceHomeVM
    {
        public Guid Id { get; set; }

        public List<EcommerceHomeCategoryVM> Categories { get; set; }

        public List<CustomItem> CustomItems { get; set; }

        public EcommerceHomeVM()
        {
            Categories = new List<EcommerceHomeCategoryVM>();
        }

        public EcommerceHomeVM(EcommerceHome home)
        {
            Id = home.Id;
            Categories = home.Categories?.Select(x => new EcommerceHomeCategoryVM(x)).ToList() ?? new List<EcommerceHomeCategoryVM>();
        }
    }

    public class EcommerceHomeCategoryVM
    {
        public Guid Id { get; set; }

        public Guid EcommerceHomeId { get; set; }

        [DisplayNameLocalized("ECOMMERCE_HOME_ORDER_LABEL")]
        public int Order { get; set; }

        [DisplayNameLocalized("ECOMMERCE_HOME_CATEGORY_ID_LABEL")]
        [RequiredIfLocalized("RequiredCategoryId", "ECOMMERCE_HOME_CATEGORY_ID_ERROR")]
        public int? ProductCategoryId { get; set; }

        public string CategoryName { get; set; }

        public Guid? CustomCategoryId { get; set; }

        [DisplayNameLocalized("ECOMMERCE_HOME_CUSTOM_CATEGORY_NAME_LABEL")]
        [RequiredIfLocalized("UseCustomCategory", "ECOMMERCE_HOME_CUSTOM_CATEGORY_ID_ERROR")]
        public string CustomCategoryName { get; set; }

        public bool? CarouselDisplay { get; set; }

        public bool IsEdit { get; set; }

        public bool UseCustomCategory { get; set; }

        public bool RequiredCategoryId { get { return !UseCustomCategory; } }

        public List<CustomItemViewModel> CustomItems { get; set; } = new List<CustomItemViewModel>();

        public bool HasCustomItems => CustomItems.Any();

        public EcommerceHomeCategoryVM()
        {
        }

        public EcommerceHomeCategoryVM(EcommerceHomeCategory category)
        {
            Id = category.Id;
            EcommerceHomeId = category.EcommerceHomeId;
            Order = category.Order;
            ProductCategoryId = category.ProductCategoryId;
            CategoryName = category.CustomItemCategory?.Name ?? category.ProductCategory?.Name ?? "";
            CustomCategoryId = category.CustomItemCategoryId;
            UseCustomCategory = category.CustomItemCategoryId.HasValue;
            CarouselDisplay = category?.CustomItemCategory?.CarouselDisplay ?? false;
            CustomCategoryName = category?.CustomItemCategory?.Name ?? null;
        }

        public void UpdateDomainModel(EcommerceHomeCategory domainCategory)
        {
            domainCategory.Order = Order;
            domainCategory.ProductCategoryId = ProductCategoryId;

            if (UseCustomCategory)
                domainCategory.CustomItemCategoryId = CustomCategoryId;
            else
                domainCategory.CustomItemCategoryId = null;
        }
    }
}