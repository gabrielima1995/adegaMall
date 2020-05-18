namespace NewIntegrator.Stripe.Customers
{
    public class StripeCustomer
    {
        public string Id { get; internal set; }
        public string Email { get; internal set; }
        public string Name { get; internal set; }
        public string Phone { get; internal set; }
    }
}