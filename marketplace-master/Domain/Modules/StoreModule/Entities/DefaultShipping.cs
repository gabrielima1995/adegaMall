using System;

namespace Domain.Modules.StoreModule.Entities
{
    public class DefaultShipping
    {
        private decimal amount;
        private int deliveryDays;

        public decimal Amount
        {
            get => amount;
            set { ThrowIfInvalidAmount(value); amount = value; }
        }
        public int DeliveryDays
        {
            get => deliveryDays;
            set { ThrowIfInvalidDeliveryDays(value); deliveryDays = value; }
        }

        public bool IsConfigured => Amount > 0 && DeliveryDays > 0;

        private void ThrowIfInvalidAmount(decimal value)
        {
            if (value < 0) throw new ArgumentException("O valor do frete padrão não pode ser menor que zero.");
        }

        private void ThrowIfInvalidDeliveryDays(int value)
        {
            if (value < 0) throw new ArgumentException("O quantidade de dias para entrega do frete padrão não pode ser menor que zero.");
        }
    }
}
