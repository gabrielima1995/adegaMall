using Domain.Modules.StoreModule.Entities;
using System;

namespace Domain.DTO
{
    /// <summary>
    /// Representa um horário de locação.
    /// </summary>
    public class RentalPeriodGridView
    {
        /// <summary>
        /// Horário do dia.
        /// </summary>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Rótulo do horário para exibir.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Total de produtos disponíveis para locação neste horário.
        /// </summary>
        public int CurrentStock { get; private set; }

        /// <summary>
        /// Total de produtos disponíveis para locação e em locação neste horário.
        /// </summary>
        public int OriginalStock { get; private set; }

        /// <summary>
        /// Percentual de desconto.
        /// </summary>
        public int Discount { get; private set; }

        /// <summary>
        /// Identificador da disponibilidade do produto.
        /// </summary>
        public Guid? Availability { get; private set; }

        /// <summary>
        /// Indica se a disponibilidade do produto já foi excluída.
        /// </summary>
        public bool IsAvailabilityDeleted { get; set; }

        public bool OutOfStock => CurrentStock <= 0;

        public int UnavailableStock => OriginalStock - CurrentStock;

        protected RentalPeriodGridView()
        {
        }

        /// <summary>
        /// Novo horário locação.
        /// </summary>
        /// <param name="time">Horário do dia.</param>
        /// <param name="currentStock">Total de produtos disponíveis para locação neste horário.</param>
        /// <param name="originalStock">Total de produtos disponíveis para locação e em locação neste horário.</param>
        /// <param name="discount">Percentual de desconto.</param>
        /// <param name="availability">Identificador da disponibilidade do produto.</param>
        protected RentalPeriodGridView(TimeSpan time, int currentStock, int originalStock, int discount, Guid? availability, bool isAvailabilityDeleted)
        {
            Time = time;
            Label = time.ToString(@"hh\:mm");
            CurrentStock = currentStock;
            OriginalStock = originalStock;
            Discount = discount;
            Availability = availability;
            IsAvailabilityDeleted = isAvailabilityDeleted;
        }

        /// <summary>
        /// Novo horário locação.
        /// </summary>
        /// <param name="time">Horário do dia.</param>
        /// <param name="availability">Disponibilidade do produto.</param>
        public static RentalPeriodGridView New(TimeSpan time, ProductAvailability availability)
        {
            return new RentalPeriodGridView(time, availability.CurrentStock, availability.CurrentStock, availability.DiscountPercentage, availability.Id, availability.Deleted);
        }

        /// <summary>
        /// Novo horário locação.
        /// </summary>
        /// <param name="time">Horário do dia.</param>
        /// <param name="currentStock">Total de produtos disponíveis para locação neste horário.</param>
        public static RentalPeriodGridView New(TimeSpan time, int currentStock)
        {
            return new RentalPeriodGridView(time, currentStock, currentStock, 0, null, false);
        }

        /// <summary>
        /// Remove determinada quantidade do estoque.
        /// </summary>
        /// <param name="amount">Quantidade a ser removida.</param>
        public void SubstractStock(int amount)
        {
            CurrentStock -= amount;
        }

        public override string ToString()
        {
            return Label;
        }
    }
}