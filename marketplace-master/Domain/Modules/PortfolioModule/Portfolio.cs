using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Events;
using Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Modules.Portfolio
{
    /// <summary>
    /// Portfólio de um vendedor
    /// </summary>
    public class Portfolio : IEntity
    {
        private readonly List<DomainEvent> domainEvents = new List<DomainEvent>();

        [Key]
        [ForeignKey("Seller")]
        public string SellerId { get; set; }

        public virtual Seller Seller { get; set; }

        public IList<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

        public IList<SellerSocialMedia> SocialMedia { get; set; } = new List<SellerSocialMedia>();

        public IList<PortfolioItem> Items { get; set; } = new List<PortfolioItem>();

        public IReadOnlyCollection<DomainEvent> DomainEvents => domainEvents.AsReadOnly();

        public void AddDomainEvent(DomainEvent domainEvent) => domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => domainEvents.Clear();

        public object GetId() => SellerId;
    }
}