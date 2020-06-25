using Application.Config;
using Domain.Entities;
using Domain.Validation.Specifications;
using NewContext.Queries.Buyers;
using Presentation.Areas.Admin.Models;
using System;
using System.Collections.Generic;

namespace Presentation.Areas.Admin.Views.Buyers
{
    public class IndexModel : BaseAdminModel
    {
        public IndexModel(IEnumerable<BuyerListItemModel> buyers)
        {
            Buyers = buyers;
        }

        public IEnumerable<BuyerListItemModel> Buyers { get; }
    }

    public class BuyerListItemModel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string CityState { get; private set; }
        public int OrdersCount { get; private set; }
        public decimal OrdersTotalAmount { get; private set; }
        public int QuotesCount { get; private set; }
        public DateTime RegisteredAt { get; private set; }
        public bool Enabled { get; private set; }
        public bool IsCompleteBuyer { get; set; }
        public bool RequireModeration { get; private set; }

        protected BuyerListItemModel()
        {
        }

        public static BuyerListItemModel From(ListBuyersForAdmin.BuyerDto dto)
        {
            var model = new BuyerListItemModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                CityState = dto.CityAndState.HasValue ? dto.CityAndState.Value : "-",
                OrdersCount = dto.OrdersCount,
                OrdersTotalAmount = dto.OrdersTotalAmount,
                QuotesCount = dto.QuotesCount,
                RegisteredAt = dto.RegisteredAt,
                Enabled = dto.Enabled,
                RequireModeration = MaaSConfig.Current.Buyer.Moderation.Enabled && dto.RequireModeration
            };

            model.IsCompleteBuyer = new CompleteBuyerSpecification()
                .IsSatisfiedBy(new CompleteBuyerSpecificationDto(model.RequireModeration));

            return model;
        }
    }

    public class BuyerModel : IWithCustomFields
    {
        public Buyer Buyer { get; set; }
        public CustomFieldEntityType EntityType => CustomFieldEntityType.Buyer;
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }

        public Guid Id { get => new Guid(Buyer.Id); set { } }
    }
}