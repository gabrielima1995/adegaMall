using System;
using Domain.Entities.StoreModule;

namespace Domain.DTO
{
    public class ProductGetting
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Specifications { get; set; }
        public decimal? Price { get; set; }
        public decimal? CurrentStock { get; set; }
        public string CustomId { get; set; }

        public ProductGetting()
        {
            
        }

        public ProductGetting(Product o)
        {
            Id = o.Id;
            Title = o.Title;
            Description = o.Description;
            Specifications = o.Specifications;
            Price = o.Price;
            CurrentStock = o.CurrentStock;
            CustomId = o.CustomId;
        }
    }
}