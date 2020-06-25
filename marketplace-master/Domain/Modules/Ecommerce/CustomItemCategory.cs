using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    /// <summary>
    /// Categoria de produto
    /// </summary>
    public class CustomItemCategory : SoftDeletableEntity<Guid>
    {
        public string Name { get; set; }

        public bool CarouselDisplay { get; set; }

        public IList<CustomItem> Products { get; set; }

        public CustomItemCategory()
        {
            this.Id = Guid.NewGuid();
        }

        public CustomItemCategory(string name)
        {
            Name = name;
        }
    }
}