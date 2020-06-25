using Application.Config;
using Application.Config.Modules.Product;
using Domain.Entities.StoreModule;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Products.Store
{
    public class SingleFreightParameterViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Raio de entrega.
        /// </summary>
        [RequiredLocalized]
        [RangeLocalized(1, 3000)]
        [DisplayNameLocalized("PRODUCT_DELIVERY_RANGE_LABEL")]
        public int Range { get; set; }

        /// <summary>
        /// Valor do frete.
        /// </summary>
        [RequiredIfLocalized("RequireShippingAmount")]
        [RangeLocalized(0D, 100000D)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [DisplayNameLocalized("PRODUCT_SHIPPING_LABEL")]
        public decimal Shipping { get; set; }

        public bool RequireShippingAmount => MaaSConfig.Current.Product.PrivateFleetShipping.Enabled && !MaaSConfig.Current.Product.PrivateFleetShipping.AllowFreeShipping;

        public SingleFreightParameterViewModel()
        {
            Id = Guid.NewGuid();
        }

        public SingleFreightParameterViewModel(SingleFreightParameter entity)
        {
            Id = entity.Id;
            Range = (int)entity.DeliveryRange;
            Shipping = entity.Shipping;
        }

        public SingleFreightParameterViewModel(PrivateFleetParameterConfig config)
        {
            Id = Guid.NewGuid();
            Range = config.Range;
        }

        public SingleFreightParameter Create()
        {
            return new SingleFreightParameter
            {
                DeliveryRange = Range,
                Shipping = Shipping
            };
        }
    }
}