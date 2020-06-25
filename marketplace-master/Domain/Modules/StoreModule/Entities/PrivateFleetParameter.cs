using Framework;
using System;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Parâmetros de Frota privada/própria para determinada distância para um produto.
    /// </summary>
    public class PrivateFleetParameter : BaseEntity<Guid>
    {
        /// <summary>
        /// Raio de entrega.
        /// </summary>
        public double DeliveryRange { get; set; }

        /// <summary>
        /// Valor do frete.
        /// </summary>
        public decimal Shipping { get; set; }

        public Product Product { get; set; }

        public PrivateFleetParameter()
        {
            Id = Guid.NewGuid();
        }

        public static PrivateFleetParameter Create(Guid id, double range, decimal price)
        {
            Guard.Ensure.NotEmpty(id, nameof(id));
            Guard.Ensure.GreaterThan(range, 0D, nameof(range));
            Guard.Ensure.GreaterOrEqual(price, 0M, nameof(price));

            return new PrivateFleetParameter
            {
                Id = id,
                DeliveryRange = range,
                Shipping = price
            };
        }
    }
}