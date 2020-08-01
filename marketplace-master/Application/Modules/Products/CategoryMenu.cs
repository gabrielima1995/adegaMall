using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Application.Modules.Products
{
    public class CategoryMenu
    {
        public ProductCategory Category { get; private set; }

        public string Title { get; private set; }
        public string Description { get; private set; }

        public List<CategoryMenuItem> Items { get; private set; }

        public CategoryMenu(ProductCategory category,
            string title, string description,
            List<CategoryMenuItem> items)
        {
            Category = category;
            Title = title;
            Description = description;
            Items = items;
        }
    }
}