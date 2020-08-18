using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Validation.Specifications;
using NewContext.Queries.Sellers;
using Presentation.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities.StoreModule;

namespace Presentation.Areas.Admin.Views.Sellers
{
    public class IndexModel : BaseAdminModel
    {
        public IndexModel(IEnumerable<SellerListItemModel> sellers)
        {
            Sellers = sellers;
        }

        public IEnumerable<SellerListItemModel> Sellers { get; }
        public bool ShowGlobalFees => Config.Payment.Enabled && Config.Product.Enabled;
    }

    public class SellerListItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string WhatsAppNumber { get; set; }
        public string AlternativeName { get; set; }
        public string Email { get; set; }
        public string CityState { get; private set; }
        public bool HasActiveSubscription { get; set; }
        public int OrdersCount { get; private set; }
        public decimal OrdersTotalAmount { get; private set; }
        public int QuotesCount { get; private set; }
        public DateTime RegisteredAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }
        public bool Enabled { get; private set; }
        public bool EnableCustomFees { get; private set; }
        public bool CustomIsLiable { get; set; }
        public decimal CustomPaymentFeePercent { get; set; }
        public decimal CustomPaymentFeeAmount { get; set; }
        public bool IsCompleteSeller { get; set; }
        public bool RequireModeration { get; set; }

        public string CompleteProfile { get; set; }

        public SellerListItemModel()
        {
        }

        public static SellerListItemModel From(Seller seller)
        {
            var model = new SellerListItemModel();

            model.Id = seller.Id;
            model.Name = seller.Name;
            model.Avatar = seller.Avatar;
            model.WhatsAppNumber = seller.WhatsAppNumber;
            model.AlternativeName = seller.AlternativeName ?? seller.Name;
            model.Email = seller.Email;
            model.CityState = seller.Address?.CityAndState;
            model.HasActiveSubscription = seller.HasSubscription && seller.IsValidSubscription;
            model.OrdersCount = seller.Orders.Count(i => i.Status != OrderStatus.Canceled);
            model.OrdersTotalAmount = seller.Orders.Where(i => i.Status != OrderStatus.Canceled).Sum(x => x.Amount);
            model.QuotesCount = seller.Quotes.Count;
            model.RegisteredAt = seller.CreatedAt;
            model.ModifiedAt = seller.ModifiedAt;
            model.Enabled = seller.Enabled;
            model.EnableCustomFees = seller.OverrideGlobalPaymentFees.GetValueOrDefault();
            model.CustomIsLiable = seller.CustomIsLiable.GetValueOrDefault();
            model.CustomPaymentFeePercent = seller.CustomPaymentFeePercent.GetValueOrDefault();
            model.CustomPaymentFeeAmount = seller.CustomPaymentFeeAmount.GetValueOrDefault();
            model.IsCompleteSeller = seller.SellerCompleteProfileMapped.IsComplete;
            model.CompleteProfile = seller.SellerCompleteProfile;
            model.RequireModeration = MaaSConfig.Current.Seller.Moderation.Enabled && seller.RequireModeration.GetValueOrDefault();

            return model;
        }

        public static SellerListItemModel From(ListSellersPendingModerationForAdmin.SellerPendingModerationDto dto)
        {
            var model = new SellerListItemModel();

            model.Id = dto.Id;
            model.Name = dto.Name;
            model.Avatar = dto.Avatar;
            model.WhatsAppNumber = dto.WhatsAppNumber;
            model.AlternativeName = dto.AlternativeName;
            model.Email = dto.Email;
            model.CityState = dto.CityState.HasValue ? dto.CityState.Value : "-";
            model.HasActiveSubscription = dto.HasActiveSubscription;
            model.OrdersCount = dto.OrdersCount;
            model.OrdersTotalAmount = dto.OrdersTotalAmount;
            model.QuotesCount = dto.QuotesCount;
            model.RegisteredAt = dto.RegisteredAt;
            model.ModifiedAt = dto.ModifiedAt;
            model.Enabled = dto.Enabled;
            model.EnableCustomFees = dto.EnableCustomFees;
            model.CustomIsLiable = dto.CustomIsLiable;
            model.CustomPaymentFeePercent = dto.CustomPaymentFeePercent;
            model.CustomPaymentFeeAmount = dto.CustomPaymentFeeAmount;
            model.RequireModeration = MaaSConfig.Current.Seller.Moderation.Enabled && dto.RequireModeration;

            model.IsCompleteSeller = dto.SellerCompleteProfileMapped.IsComplete;
            model.CompleteProfile = dto.SellerCompleteProfile;

            return model;
        }
    }

    public class SellerModel : IWithCustomFields
    {
        public Seller Seller { get; set; }
        public CustomFieldEntityType EntityType => CustomFieldEntityType.Seller;
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public Guid Id { get => new Guid(Seller.Id); set { } }
        public List<SellerScheduledTransferViewModel> SellerScheduledTransfer { get; set; }
    }
}