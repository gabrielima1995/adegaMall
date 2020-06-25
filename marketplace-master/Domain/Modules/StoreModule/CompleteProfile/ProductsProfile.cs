namespace Domain.Modules.StoreModule.CompleteProfile
{
    public class ProductsProfile
    {
        public bool IsComplete { get; set; }

        public bool NeedsRegistration { get; set; }

        public int Quantity { get; set; } = 0;
    }
}
