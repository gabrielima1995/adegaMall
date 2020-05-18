using Domain.Entities;
using System;

namespace Domain.Modules.ProductTemplateModule
{
    public class ProductTemplateImage : BaseEntity<Guid>
    {
        private ProductTemplateImage(/*Ef*/)
        {
        }

        public ProductTemplateImage(string url, int position) : this(Guid.NewGuid(), url, position)
        {
        }

        public ProductTemplateImage(Guid id, string url, int position)
        {
            Id = id;
            Url = url;
            Position = position;
        }

        public string Url { get; private set; }
        public int Position { get; private set; }
        public Guid ProductTemplateId { get; set; }
    }
}
