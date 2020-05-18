namespace System
{
    public static class DecimalExtensions
    {
        public static decimal RoundTo2Places(this decimal input)
        {
            return Math.Round(input, 2, MidpointRounding.AwayFromZero);
        }

        public static int ToCents(this decimal value)
        {
            return (int)(value * 100);
        }
    }
}
