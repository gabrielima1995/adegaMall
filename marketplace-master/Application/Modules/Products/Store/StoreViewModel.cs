using Application.Config;
using Domain.Entities.StoreModule;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Application.Modules.Products.Store
{
    /// <summary>
    /// View model de gestão da "vitrine".
    /// </summary>
    public class StoreViewModel
    {
        public string SellerId { get; set; }

        public IList<Product> Products { get; set; }

        [DisplayNameLocalized("STORE_SINGLE_FREIGHT_METHOD_LABEL")]
        public bool SingleFreightEnabled { get; set; }

        public IList<SingleFreightParameterViewModel> SingleFreightParams { get; set; } = new List<SingleFreightParameterViewModel>();

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

        [DisplayNameLocalized("STORE_TOTALEXPRESS_METHOD_LABEL")]
        public bool TotalExpressEnabled { get; set; }

        [DisplayNameLocalized("STORE_TOTALEXPRESS_USER_LABEL")]
        public string TotalExpressUser { get; set; }

        [DisplayNameLocalized("STORE_TOTALEXPRESS_PASSWORD_LABEL")]
        public string TotalExpressPassword { get; set; }
        
        [DisplayNameLocalized("STORE_TOTALEXPRESS_REID_LABEL")]
        public string TotalExpressReid { get; set; }

        public StoreViewModel()
        {
            Products = new List<Product>();

            SingleFreightEnabled = MaaSConfig.Current.Product.ShippingMode == ShippingMode.PerStore;

            SingleFreightParams = MaaSConfig.Current.Product.PrivateFleetShipping.DefaultParams
                .Select(i => new SingleFreightParameterViewModel(i))
                .ToList();
        }

        public StoreViewModel(Domain.Entities.StoreModule.Store store)
        {
            SellerId = store.SellerId;

            Products = (store?.Products ?? new List<Product>())
                .Where(i => !i.Deleted)
                .ToList();

            SingleFreightEnabled = store.PrivateFleetEnabled;

            if (store.SingleFreightParams != null && store.SingleFreightParams.Count > 0)
            {
                SingleFreightParams = store.SingleFreightParams.Select(x => new SingleFreightParameterViewModel(x)).ToList();
            }
            else
            {
                SingleFreightParams = MaaSConfig.Current.Product.PrivateFleetShipping.DefaultParams
                    .Select(i => new SingleFreightParameterViewModel(i))
                    .ToList();
            }

            OnSitePickupEnabled = store.OnSitePickupEnabled;
            SedexEnabled = store.SedexEnabled;
            PacEnabled = store.PacEnabled;
            AsapLogEnabled = store.AsapLogEnabled;
            AsapLogUser = store.AsapLogUser;
            AsapLogPassword = store.AsapLogPassword;
            TotalExpressEnabled = store.TotalExpressEnabled;
            TotalExpressUser = store.TotalExpressUser;
            TotalExpressPassword = store.TotalExpressPassword;
            TotalExpressReid = store.TotalExpressReid;
        }

        public Domain.Entities.StoreModule.Store Create()
        {
            var entity = new Domain.Entities.StoreModule.Store();

            entity.SellerId = SellerId;

            #region Shipping

            entity.PrivateFleetEnabled = SingleFreightEnabled;

            if (SingleFreightEnabled)
            {
                SingleFreightParams = SingleFreightParams?
                    .DistinctBy(i => i.Range)
                    .ToList();

                SingleFreightParams.ForEach(sfp =>
                {
                    entity.SingleFreightParams.Add(sfp.Create());
                });
            }

            entity.OnSitePickupEnabled = OnSitePickupEnabled;
            entity.SedexEnabled = SedexEnabled;
            entity.PacEnabled = PacEnabled;
            entity.AsapLogEnabled = AsapLogEnabled;
            entity.AsapLogUser = AsapLogUser;
            entity.AsapLogPassword = AsapLogPassword;
            entity.TotalExpressEnabled = TotalExpressEnabled;
            entity.TotalExpressUser = TotalExpressUser;
            entity.TotalExpressPassword = TotalExpressPassword;
            entity.TotalExpressReid = TotalExpressReid;

            #endregion

            return entity;
        }

        public void Update(Domain.Entities.StoreModule.Store store)
        {
            if (Products != null && Products.Count > 0)
            {
                store.Products = Products;
            }

            #region Shipping

            store.PrivateFleetEnabled = SingleFreightEnabled;

            store.SingleFreightParams.Clear();

            if (SingleFreightEnabled)
            {
                SingleFreightParams = SingleFreightParams?
                    .DistinctBy(i => i.Range)
                    .ToList();

                SingleFreightParams.ForEach(sfp =>
                {
                    store.SingleFreightParams.Add(sfp.Create());
                });
            }

            store.OnSitePickupEnabled = OnSitePickupEnabled;
            store.SedexEnabled = SedexEnabled;
            store.PacEnabled = PacEnabled;
            store.AsapLogEnabled = AsapLogEnabled;
            store.AsapLogUser = AsapLogUser;
            store.AsapLogPassword = AsapLogPassword;
            store.TotalExpressEnabled = TotalExpressEnabled;
            store.TotalExpressUser = TotalExpressUser;
            store.TotalExpressPassword = TotalExpressPassword;
            store.TotalExpressReid = TotalExpressReid;

            #endregion
        }
    }
}