namespace PaymentIntegrator.Moip.Helpers
{
    public static class PlanHelper
    {
        public static int ToDays(string unit, int length)
        {
            switch (unit.ToLowerInvariant())
            {
                case "month":
                    return length * 30;
                case "year":
                    return length * 365;
                case "day":
                default:
                    return length;
            }
        }
    }
}
