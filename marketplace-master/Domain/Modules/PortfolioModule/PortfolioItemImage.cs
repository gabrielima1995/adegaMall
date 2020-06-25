using Domain.Entities;
using Framework;
using System;

namespace Domain.Modules.Portfolio
{
    /// <summary>
    /// Imagem para um item do portfólio
    /// </summary>
    public class PortfolioItemImage : BaseEntity<Guid>, IWithPosition, IImage
    {
        public string Url { get; set; }

        public int Position { get; set; }

        public PortfolioItem PortfolioItem { get; set; }

        public PortfolioItemImage()
        {
            Id = Guid.NewGuid();
        }
    }
}