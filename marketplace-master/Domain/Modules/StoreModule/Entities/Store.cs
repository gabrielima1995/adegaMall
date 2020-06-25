using Domain.Events;
using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Entities;
using Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Entities.StoreModule
{
    public class Store : IEntity
    {
        private IList<SingleFreightParameter> _singleFreightParams;
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        [Key]
        [ForeignKey("Seller")]
        public string SellerId { get; set; }

        public virtual Seller Seller { get; set; }

        public bool OnSitePickupEnabled { get; set; }

        public bool PrivateFleetEnabled { get; set; }

        public bool SedexEnabled { get; set; }

        public bool PacEnabled { get; set; }

        public DefaultShipping DefaultShipping { get; private set; } = new DefaultShipping();

        public IList<SingleFreightParameter> SingleFreightParams
        {
            get { return _singleFreightParams = _singleFreightParams?.OrderBy(i => i.DeliveryRange).ToList(); }
            set { _singleFreightParams = value; }
        }

        public RentalSettings RentalSettings { get; set; } = new RentalSettings();

        public IList<Product> Products { get; set; }

        public Store()
        {
            Products = new List<Product>();
            SingleFreightParams = new List<SingleFreightParameter>();
        }

        public object GetId() => SellerId;

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}