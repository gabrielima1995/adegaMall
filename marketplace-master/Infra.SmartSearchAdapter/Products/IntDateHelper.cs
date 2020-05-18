using System;

namespace Infra.SmartSearchAdapter.Products
{
    internal static class IntDateHelper
    {
        public static int Convert(DateTime dateTime)
        {
            return
                dateTime.Year * 10000 +
                dateTime.Month * 100 +
                dateTime.Day;
        }

        public static DateTime Convert(int yyyymmdd)
        {
            int d = yyyymmdd % 100;
            int m = yyyymmdd / 100 % 100;
            int y = yyyymmdd / 10000;
            return new DateTime(y, m, d);
        }
    }
}
