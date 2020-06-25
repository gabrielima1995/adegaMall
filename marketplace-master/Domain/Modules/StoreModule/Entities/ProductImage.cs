using Framework;
using System;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Imagem/foto de um produto.
    /// </summary>
    public class ProductImage : BaseEntity<Guid>, IWithPosition, IImage
    {
        public string Url { get; set; }

        public int Position { get; set; }

        public Product Product { get; set; }

        public ProductImage()
        {
            Id = Guid.NewGuid();
        }
    }
}