using Domain.Entities.StoreModule;
using System;

namespace Domain.Entities
{
    public class EcommerceHomeCategory : SoftDeletableEntity<Guid>
    {
        public EcommerceHome EcommerceHome { get; set; }

        public Guid EcommerceHomeId { get; set; }

        public ProductCategory ProductCategory { get; set; }

        public CustomItemCategory CustomItemCategory { get; set; }

        public int? ProductCategoryId { get; set; }

        public Guid? CustomItemCategoryId { get; set; }

        public int Order { get; set; }

        public EcommerceHomeCategory()
        {
            Id = Guid.NewGuid();
        }

        public EcommerceHomeCategory(EcommerceHome home, ProductCategory category, int order)
            : this()
        {
            Order = order;
            if (home != null) EcommerceHomeId = home.Id;
            if (category != null) ProductCategoryId = category.Id;
        }
    }
}
