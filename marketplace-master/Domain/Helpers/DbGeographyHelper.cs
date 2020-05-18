using System;
using System.Data.Entity.Spatial;
using System.Globalization;

namespace Framework.Helpers
{
    public static class DbGeographyHelper
    {
        public static DbGeography FromLocation(double lat, double lng)
        {
            if (lat < -90D || lat > 90D)
                throw new ArgumentException($"Invalid latitude ({lat}). Latitude must be a number between -90 and 90 (inclusive).");

            if (lng < -180D || lng > 180D)
                throw new ArgumentException($"Invalid longitude ({lng}). Longitude must be a number between -180 and 180 (inclusive).");

            return DbGeography.FromText(string.Format(CultureInfo.InvariantCulture, "POINT({1} {0})", lat, lng));
        }
    }
}
