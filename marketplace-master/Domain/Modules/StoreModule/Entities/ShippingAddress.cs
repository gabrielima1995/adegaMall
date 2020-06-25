using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Relacionamento entre endereço de entrega e pessoa.
    /// </summary>
    public class ShippingAddress : BaseEntity<Guid>
    {
        [Required]
        public Person Person { get; set; }

        [Required]
        public Address Address { get; set; }

        public ShippingAddress()
        {
            Id = Guid.NewGuid();
        }
    }
}