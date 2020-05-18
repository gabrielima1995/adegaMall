namespace PaymentIntegrator
{
    public static class PaymentHelper
    {
        public static int ToCents(this decimal amount)
        {
            return (int)(amount * 100);
        }

        public static decimal FromCents(this int i)
        {
            return i / 100M;
        }
    }
}
