using Domain.Entities;
using Domain.Entities.StoreModule;

namespace Domain.Events
{
    /// <summary>
    /// Evento referente a mudança de status de um pedido.
    /// </summary>
    public class OrderStatusChangedEvent : DomainEvent
    {
        /// <summary>
        /// Pedido.
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Novo status do pedido.
        /// </summary>
        public OrderStatus NewStatus { get; }

        /// <summary>
        /// Pessoa responsável por mudar o status do pedido.
        /// </summary>
        public Person ActingPerson { get; }

        /// <summary>
        /// Dados adicionais.
        /// </summary>
        public string AdditionalInfo { get; }

        public OrderStatusChangedEvent(Order order, OrderStatus newStatus, Person actingPerson, string additionalInfo)
        {
            Order = order;
            NewStatus = newStatus;
            ActingPerson = actingPerson;
            AdditionalInfo = additionalInfo;
        }
    }
}