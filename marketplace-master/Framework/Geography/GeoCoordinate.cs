using System.Globalization;

namespace Framework.Geography
{
    public class GeoCoordinate
    {
        public double Lat { get; private set; }

        public double Lng { get; private set; }

        public GeoCoordinate(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }

        public override string ToString()
        {
            var culture = CultureInfo.InvariantCulture;
            return $"({Lat.ToString(culture)}, {Lng.ToString(culture)})";
        }

        public string ToWellKnownText()
        {
            var culture = CultureInfo.InvariantCulture;
            return $"POINT ({Lng.ToString(culture)} {Lat.ToString(culture)})";
        }
    }
}
