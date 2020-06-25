using System;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Histórico do pedido.
    /// </summary>
    public class OrderHistory : BaseEntity<Guid>
    {
        /// <summary>
        /// Pedido relacionado.
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Data da ocorrência.
        /// </summary>
        public DateTime OccurredOn { get; set; }

        /// <summary>
        /// Status do pedido na ocorrência.
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Pessoa relacionada à ocorrência.
        /// </summary>
        public Person Person { get; set; }

        public string AdditionalInfo { get; set; }

        public OrderHistory()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }

        public OrderHistory(OrderStatus status, Person person = null, string additionalInfo = null)
            : this()
        {
            Status = status;
            Person = person;
            AdditionalInfo = additionalInfo;
        }
    }
}