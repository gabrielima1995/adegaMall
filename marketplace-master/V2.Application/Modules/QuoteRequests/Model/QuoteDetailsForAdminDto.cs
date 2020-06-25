using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System;
using System.Collections.Generic;
using Domain.QuoteRequests;

namespace V2.Application.Modules.QuoteRequests.Model
{
    public class QuoteDetailsForAdminDto : IWithCustomFields
    {
        public Buyer Buyer { get; set; }

        public Seller Seller { get; set; }

        public Address Address { get; set; }

        public Quote Quote { get; set; }

        public bool DealWon => Quote.DealResult == QuoteDealResult.Won;
        public bool IsClosed => Quote.IsClosed;
        public string SellerUrl => Seller?.FriendlyUrl ?? "";

        public IEnumerable<QuoteAnswer> Answers { get; set; }

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Quote;

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public Guid Id
        {
            get { return Quote.Id; }
            set { }
        }
    }
}
