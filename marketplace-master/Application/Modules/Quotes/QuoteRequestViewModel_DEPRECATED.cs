using Application.Config;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Quotes
{
    [Obsolete]
    public class QuoteRequestViewModel_DEPRECATED : IWithAddress, IWithCustomFields
    {
        public string SellerId { get; set; }

        public string SellerNickname { get; set; }

        [DisplayNameLocalized("QUOTE_REGISTER_PASSWORD")]
        public string NewPassword { get; set; }

        [DisplayNameLocalized("QUOTE_REGISTER_PASSWORD_REPEAT")]
        public string ConfirmPassword { get; set; }

        [DisplayNameLocalized("NAME_LABEL")]
        [RequiredLocalized]
        public string Name { get; set; }

        [DisplayNameLocalized("EMAIL_LABEL")]
        [RequiredLocalized]
        [EmailAddressLocalized]
        public string Email { get; set; }

        [DisplayNameLocalized("QUOTE_TITLE_LABEL")]
        [RequiredLocalized]
        public string Title { get; set; }

        [DisplayNameLocalized("DESCRIPTION_LABEL")]
        [RequiredLocalized]
        public string Description { get; set; }

        [DisplayNameLocalized("CATEGORY_LABEL")]
        [RequiredLocalized]
        public int? Category { get; set; }

        [DisplayNameLocalized("SUB_CATEGORIES_LABEL")]
        [RequiredLocalized]
        public int? SubCategory { get; set; }

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("ADDRESS_LABEL")]
        public string Address { get; set; }

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("ADDRESS_NUMBER_LABEL")]
        public string AddressNumber { get; set; }

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("NEIGHBORHOOD_LABEL")]
        public string AddressNeighborhood { get; set; }

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("ZIPCODE_LABEL")]
        public string AddressZipCode { get; set; }

        [RequiredIfLocalized("AddressRequired || CityAndStateRequired")]
        [DisplayNameLocalized("CITY_AND_STATE_LABEL")]
        public string CityAndState { get; set; }

        [RequiredIfLocalized("PhoneRequired")]
        [DisplayNameLocalized("PHONE_LABEL")]
        public string Phone { get; set; }

        [RequiredIfLocalized("DateRequired")]
        [DisplayNameLocalized("QUOTE_DATE_LABEL")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DisplayNameLocalized("QUOTE_PREFERENCE_LABEL")]
        public QuotePreference Preference { get; set; }

        [RequiredIfLocalized("QuoteMaxRequired")]
        [DisplayNameLocalized("QUOTE_MAX_BUDGET_LABEL")]
        public decimal? Quote { get; set; }

        public bool PhoneRequired => MaaSConfig.Current.Quote.Phone.IsRequired;
        public bool DateRequired => MaaSConfig.Current.Quote.Date.IsRequired;
        public bool AddressRequired => MaaSConfig.Current.Quote.Address.IsRequired;
        public bool QuoteMaxRequired => MaaSConfig.Current.Quote.QuoteMax.IsRequired;
        public bool CityAndStateRequired => MaaSConfig.Current.Quote.CityAndState.IsRequired;

        // Necessário na interface (contrato de implementação), mas não utilizado no Orçamento.
        public string AddressAdditionalInfo { get; set; }

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }

        public Dictionary<string, string> CustomFields { get; set; }

        public Guid Id { get; set; }

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Quote;
    }
}