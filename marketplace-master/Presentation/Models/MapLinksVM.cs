using System.Globalization;

namespace Presentation.Models
{
    public class MapLinksVM
    {
        public string Search { get; private set; }
        public string Directions { get; private set; }

        public MapLinksVM(string name, string address, double? lat, double? lng)
        {
            var cult = CultureInfo.InvariantCulture;
            string query = null;

            if (lat != null && lng != null)
                query = $"{lat.Value.ToString(cult)},{lng.Value.ToString(cult)}";

            query = query ?? address ?? name;

            if (query != null)
            {
                Search = $"https://www.google.com/maps/search/?api=1&query={query}";
                Directions = $"https://www.google.com/maps/dir/?api=1&destination={query}&travelmode=driving";
            }
        }
    }
}