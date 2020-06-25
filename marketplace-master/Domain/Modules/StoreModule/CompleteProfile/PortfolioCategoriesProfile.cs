namespace Domain.Modules.StoreModule.CompleteProfile
{
    public class PortfolioCategoriesProfile
    {
        public bool IsComplete { get; set; }

        public bool NeedsRegistration { get; set; }

        public int Quantity { get; set; } = 0;
    }
}