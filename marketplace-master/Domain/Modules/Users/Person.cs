using Domain.Entities.StoreModule;
using Domain.Events;
using Framework;
using Infra.Crosscutting.Globalization.Attributes;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Suppliers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public abstract class Person : ISupplier, ICustomer, IEntity
    {
        private string lang;

        private const string defaultTimeZoneId = "E. South America Standard Time";
        private string preferredTimeZoneId;
        private MarketplaceTimeZone preferredTimeZone;

        protected Person() { }
        protected Person(string userId, string email, bool requireModeration) : this()
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

            Id = UserOrPersonId.Generate();
            UserId = userId;
            Email = UserName = email;
            RequireModeration = requireModeration;
            
            Enabled = true;
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        public string Id { get; private set; }
        public string UserId { get; private set; }
        
        [StringLength(256)]
        public string Email { get; private set; }

        [StringLength(256)]
        public string UserName { get; private set; }

        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; set; }

        public string PhoneNumber { get; set; }
        
        public string FullName { get; set; }
        
        public string Avatar { get; set; }
        
        public DateTime? DateOfBirth { get; set; }

        public Genre? Genre { get; set; }

        public string DocumentNumber { get; set; }

        public string AdditionalInfo1 { get; set; }

        public string AdditionalInfo2 { get; set; }

        /// <summary>
        /// Identificador no gateway de pagamento antigo.
        /// </summary>
        public string GatewayIdOld { get; set; }

        /// <summary>
        /// Identificador no gateway de pagamento como Supplier.
        /// </summary>
        public string SupplierGatewayId { get; set; }

        /// <summary>
        /// Identificador no gateway de pagamento como Customer.
        /// </summary>
        public string CustomerGatewayId { get; set; }

        /// <summary>
        /// Identificador do usuário como assinante no Gateway
        /// </summary>
        public string SubscriberId { get { return GatewayIdOld; } }

        /// <summary>
        /// Nota de avaliação da pessoa.
        /// </summary>
        public int Rating { get; private set; }

        /// <summary>
        /// Somatório de todas as notas de avaliação recebidas.
        /// </summary>
        public int TotalRating { get; private set; }

        /// <summary>
        /// Quantidade de avaliações que a pessoa recebeu.
        /// </summary>
        public int TotalReviews { get; private set; }

        /// <summary>
        /// Score do cadastro da pessoa.
        /// </summary>
        public int Score { get; protected set; }

        public string AboutMe { get; set; }

        /// <summary>
        /// Indica se não é permitido para o usuário assinar planos trial de assinatura.
        /// </summary>
        [DefaultValue(false)]
        public bool NotAllowedTrial { get; set; }

        public Address Address { get; set; }

        public IList<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();

        /// <summary>
        /// Idioma preferido
        /// </summary>
        public string Lang
        {
            get { return lang ?? (lang = "pt-br"); }
            set
            {
                if (lang != value)
                {
                    lang = value;
                    _preferredCulture = null;
                }
            }
        }

        public CultureInfo PreferredCulture => _preferredCulture ?? (_preferredCulture = new CultureInfo(Lang));

        public string BusinessDescription => AboutMe;

        public string Phone => PhoneNumber;

        string ISupplier.GatewayId => SupplierGatewayId;

        string ICustomer.GatewayId => CustomerGatewayId;

        public string Name => FullName;

        public string AlternativeName { get; set; }

        [StringLength(50)]
        public string GatewayLiveApiToken { get; set; }

        [StringLength(50)]
        public string GatewayTestApiToken { get; set; }

        [StringLength(50)]
        public string GatewayUserToken { get; set; }

        public bool HasAvatar => Avatar != null;

        public bool HasAddress => Address != null && (Address.HasCityAndState || Address.HasFullAddress);

        public bool IntegratorSupplierRegistered => !string.IsNullOrEmpty(SupplierGatewayId);

        public bool IntegratorCustomerRegistered => !string.IsNullOrEmpty(CustomerGatewayId);

        public bool? RequireModeration { get; set; }

        public string UtmTags { get; set; }

        public virtual string AccountableDocument => null;

        public string PreferredTimeZoneId
        {
            get => preferredTimeZoneId ?? defaultTimeZoneId;
            set
            {
                if (value != preferredTimeZoneId)
                {
                    preferredTimeZoneId = value;
                    preferredTimeZone = null;
                }
            }
        }

        public MarketplaceTimeZone PreferredTimeZone => preferredTimeZone ?? (preferredTimeZone = TimeZones.Find(PreferredTimeZoneId));

        void ICustomer.SetGatewayId(string id)
        {
            CustomerGatewayId = id;
        }

        public void SetGatewayTokens(string id, string liveApiToken, string testApiToken, string userToken)
        {
            SupplierGatewayId = id;
            GatewayLiveApiToken = liveApiToken;
            GatewayTestApiToken = testApiToken;
            GatewayUserToken = userToken;
        }

        public bool IsCompany()
        {
            var regex = new Regex(@"^([0-9]{2}[\.]?[0-9]{3}[\.]?[0-9]{3}[\/]?[0-9]{4}[-]?[0-9]{2})|(\d{14})$");
            return regex.IsMatch(DocumentNumber);
        }

        public List<Address> GetAllAddresses()
        {
            var list = ShippingAddresses.Select(i => i.Address).ToList();

            if (Address != null)
                list.Insert(0, Address);

            return list;
        }

        /// <summary>
        /// Avalia a pessoa com uma nota e calcula suas médias.
        /// </summary>
        /// <param name="rate">Nota para a pessoa.</param>
        public void Review(int rate)
        {
            TotalRating += rate;
            TotalReviews++;

            Rating = TotalRating / TotalReviews;
        }

        public object GetId()
        {
            return Id;
        }

        #region Domain Events

        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        private CultureInfo _preferredCulture;

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        #endregion Domain Events

        public string DisplayPhoneNumber()
        {
            if (string.IsNullOrEmpty(PhoneNumber))
                return PhoneNumber;

            var breakdown = new PhoneBreakdown(PhoneNumber);
            if (!breakdown.Success) return PhoneNumber;

            return $"({breakdown.Ddd}) {breakdown.Number}";
        }

        public void SetSubscriberId(string id)
        {
            GatewayIdOld = id;
        }

        [NotMapped]
        public abstract PersonType PersonType { get; }
    }

    public enum PersonType
    {
        [DescriptionLocalized("PERSON_TYPE_ADMIN")]
        AdminPerson,

        [DescriptionLocalized("PERSON_TYPE_BUYER")]
        Buyer,

        [DescriptionLocalized("PERSON_TYPE_SELLER")] 
        Seller,

        [DescriptionLocalized("PERSON_TYPE_SELLER_ASSISTANT")]
        SellerAssistant
    }
}