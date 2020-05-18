namespace Domain.Entities
{
    public enum WebhookEvent
    {
        All,

        OrderCreated,

        OrderStatusChanged,

        RentalAdded,

        RentalUpdated,

        RentalCancelled
    }
}
