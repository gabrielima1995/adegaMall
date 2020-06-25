using System;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Parâmetros de frete único por loja/vendedor.
    /// </summary>
    public class SingleFreightParameter : BaseEntity<Guid>
    {
        /// <summary>
        /// Raio de entrega.
        /// </summary>
        public double DeliveryRange { get; set; }

        /// <summary>
        /// Valor do frete.
        /// </summary>
        public decimal Shipping { get; set; }

        /// <summary>
        /// Loja associada.
        /// </summary>
        public Store Store { get; set; }

        public SingleFreightParameter()
        {
            Id = Guid.NewGuid();
        }
    }
}