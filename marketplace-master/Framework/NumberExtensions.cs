namespace Framework
{
    public static class NumberExtensions
    {
        public static double ToDoubleOrDefault(this string numString)
        {
            double num = default(double);

            double.TryParse(numString, out num);

            return num;
        }

        public static decimal ToDecimalOrDefault(this string numString)
        {
            decimal num = default(decimal);

            decimal.TryParse(numString, out num);

            return num;
        }

        public static int ToIntOrDefault(this string numString)
        {
            int num = default(int);

            int.TryParse(numString, out num);

            return num;
        }
    }
}
