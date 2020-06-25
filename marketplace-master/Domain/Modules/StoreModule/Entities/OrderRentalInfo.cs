using System;

namespace Domain.StoreModule.Entities
{
    public class OrderRentalInfo
    {
        /// <summary>
        /// Indica se a locação foi devolvida por completo.
        /// </summary>
        public bool? WasFullyReturned { get; set; }

        /// <summary>
        /// Data onde a locação foi devolvida por completo.
        /// </summary>
        public DateTime? FullyReturnedAt { get; set; }

        /// <summary>
        /// Indica se o lembrete de início de locação foi enviado para o comprador.
        /// </summary>
        public bool? StartReminderSent { get; set; }

        /// <summary>
        /// Indica se o lembrete de fim de locação foi enviado para o comprador.
        /// </summary>
        public bool? EndReminderSent { get; set; }
    }
}