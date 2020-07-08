using Application.Config;
using Application.Modules.Products.Store;
using Domain.DTO;
using Domain.Entities.StoreModule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Presentation.Models
{
    public class StoreVM
    {
        [DisplayNameLocalized("STORE_SINGLE_FREIGHT_METHOD_LABEL")]
        public bool SingleFreightEnabled { get; set; }

        [DisplayNameLocalized("STORE_ON_SITE_PICKUP_METHOD_LABEL")]
        public bool OnSitePickupEnabled { get; set; }

        [DisplayNameLocalized("STORE_SEDEX_METHOD_LABEL")]
        public bool SedexEnabled { get; set; }

        [DisplayNameLocalized("STORE_PAC_METHOD_LABEL")]
        public bool PacEnabled { get; set; }

        [DisplayNameLocalized("STORE_ASAPLOG_METHOD_LABEL")]
        public bool AsapLogEnabled { get; set; }

        [DisplayNameLocalized("STORE_ASAPLOG_USER_LABEL")]
        public string AsapLogUser { get; set; }

        [DisplayNameLocalized("STORE_ASAPLOG_PASSWORD_LABEL")]
        public string AsapLogPassword { get; set; }

        public string SellerId { get; set; }
        public List<SingleFreightParameterViewModel> SingleFreightParams { get; set; }

        public bool IsLiable { get; set; }
        public decimal AmountFee { get; set; }
        public decimal PercentFee { get; set; }

        public decimal DefaultShippingAmount { get; set; }
        public int DefaultShippingDeliveryDays { get; set; }
        public bool DefaultShippingIsConfigured => DefaultShippingAmount + DefaultShippingDeliveryDays > 0;

        public StoreVM()
        {
            SingleFreightParams = new List<SingleFreightParameterViewModel>();
        }

        public StoreVM(Store store)
        {
            SellerId = store.SellerId;
            IsLiable = store.Seller.IsSellerLiable();
            AmountFee = store.Seller.GetCurrentPaymentAmountFee();
            PercentFee = store.Seller.GetCurrentPaymentPercentFee();

            SingleFreightEnabled = store.PrivateFleetEnabled;

            if (store.SingleFreightParams != null && store.SingleFreightParams.Count > 0)
                SingleFreightParams = store.SingleFreightParams.Select(x => new SingleFreightParameterViewModel(x)).ToList();

            else SingleFreightParams = MaaSConfig.Current.Product.PrivateFleetShipping.DefaultParams
                .Select(i => new SingleFreightParameterViewModel(i))
                .ToList();

            OnSitePickupEnabled = store.OnSitePickupEnabled;
            SedexEnabled = store.SedexEnabled;
            PacEnabled = store.PacEnabled;
            AsapLogEnabled = store.AsapLogEnabled;
            AsapLogUser = store.AsapLogUser;
            AsapLogPassword = store.AsapLogPassword;

            DefaultShippingAmount = store.DefaultShipping.Amount;
            DefaultShippingDeliveryDays = store.DefaultShipping.DeliveryDays;
        }

        public Store ToDomainModel()
        {
            var store = new Store { SellerId = SellerId };

            store.OnSitePickupEnabled = OnSitePickupEnabled;
            store.PrivateFleetEnabled = SingleFreightEnabled;
            store.SedexEnabled = SedexEnabled;
            store.PacEnabled = PacEnabled;
            store.AsapLogEnabled = AsapLogEnabled;
            store.AsapLogUser = AsapLogUser;
            store.AsapLogPassword = AsapLogPassword;

            store.SingleFreightParams.Clear();
            if (SingleFreightEnabled)
            {
                SingleFreightParams = SingleFreightParams?.DistinctBy(i => i.Range).ToList() ?? new List<SingleFreightParameterViewModel>();
                SingleFreightParams.ForEach(param => store.SingleFreightParams.Add(param.Create()));
            }

            return store;
        }
    }

    public class DefaultShippingViewModel
    {
        public DefaultShippingViewModel()
        {
        }

        public DefaultShippingViewModel(Store store)
        {
            Amount = store.DefaultShipping.Amount;
            DeliveryDays = store.DefaultShipping.DeliveryDays;
        }

        [RangeLocalized(0, 99999)]
        [DisplayNameLocalized("PRODUCT_DEFAULT_SHIPPING_AMOUNT_LABEL")]
        public decimal? Amount { get; set; }

        [RangeLocalized(0, 90)]
        [DisplayNameLocalized("PRODUCT_DEFAULT_SHIPPING_DELIVERY_DAYS_LABEL")]
        public int? DeliveryDays { get; set; }

        public void SetStoreValues(Store target)
        {
            var amount = Math.Max(Amount.GetValueOrDefault(), 0);
            var days = Math.Max(DeliveryDays.GetValueOrDefault(), 0);

            if (amount == 0 || days == 0)
                amount = days = 0;

            target.DefaultShipping.Amount = amount;
            target.DefaultShipping.DeliveryDays = days;
        }
    }
}