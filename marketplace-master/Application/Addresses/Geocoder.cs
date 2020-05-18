using Application.Config;
using Geocoding.Google;

namespace Application.Addresses
{
    public class Geocoder : GoogleGeocoder
    {
        public Geocoder()
        {
            ApiKey = MaaSConfig.Current.Environment.GoogleServices.MapsApiKey;
        }
    }
}
