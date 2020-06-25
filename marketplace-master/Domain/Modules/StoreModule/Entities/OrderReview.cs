using System;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Avaliação de um pedido
    /// </summary>
    public class OrderReview : BaseEntity<Guid>
    {
        /// <summary>
        /// Pedido avaliado.
        /// </summary>
        public Order Order { get; private set; }

        /// <summary>
        /// Quem fez a avaliação (pode ser o comprador ou o vendedor).
        /// </summary>
        public Person Reviewer { get; private set; }

        /// <summary>
        /// Nota da avaliação (0 a 100).
        /// </summary>
        public int Rate { get; private set; }

        /// <summary>
        /// Descrição da avaliação.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Indica se a avaliação foi feita pelo vendedor.
        /// </summary>
        public bool BySeller => Reviewer.Id == Order.Seller.Id;

        /// <summary>
        /// Indica se a avaliação foi feita pelo comprador.
        /// </summary>
        public bool ByBuyer => Reviewer.Id == Order.Buyer.Id;

        /// <summary>
        /// Indica se a avaliação foi feita por determinado usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns></returns>
        public bool ByUser(string userId) => Reviewer.Id == userId;

        public OrderReview()
        {
            Id = Guid.NewGuid();
        }

        public OrderReview(Order order, Person reviewer, int rate, string description)
            : this()
        {
            Order = order;
            Reviewer = reviewer;

            rate = rate > 100
                ? 100
                : (rate < 0
                    ? 0
                    : rate);

            Rate = rate;
            Description = description;
        }
    }
}