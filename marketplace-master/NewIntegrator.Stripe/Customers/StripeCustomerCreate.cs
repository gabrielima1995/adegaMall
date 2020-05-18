namespace NewIntegrator.Stripe.Customers
{
    public class StripeCustomerCreate
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Source { get; set; }
    }
}