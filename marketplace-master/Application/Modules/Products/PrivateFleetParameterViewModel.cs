using Application.Config.Modules.Product;
using Domain.Entities.StoreModule;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Products
{
    public class PrivateFleetParameterViewModel
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
        [RangeLocalized(0D, 100000D)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [DisplayNameLocalized("PRODUCT_SHIPPING_LABEL")]
        public decimal Shipping { get; set; }

        public PrivateFleetParameterViewModel()
        {
            Id = Guid.NewGuid();
        }

        public PrivateFleetParameterViewModel(PrivateFleetParameter entity)
        {
            Id = entity.Id;
            Range = (int)entity.DeliveryRange;
            Shipping = entity.Shipping;
        }

        public PrivateFleetParameterViewModel(PrivateFleetParameterConfig config)
        {
            Id = Guid.NewGuid();
            Range = config.Range;
        }

        public PrivateFleetParameter Create()
        {
            return new PrivateFleetParameter
            {
                DeliveryRange = Range,
                Shipping = Shipping
            };
        }
    }
}