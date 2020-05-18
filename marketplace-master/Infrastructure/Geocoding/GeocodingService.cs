using Application.Config;
using Domain.Entities;
using Domain.Interfaces;
using Framework;
using Framework.Geography;
using Infrastructure.Geocoding;
using System.Linq;

namespace Infrastructure
{
    public class GeocodingService : IGeocodingService
    {
        public OperationResult<GeoCoordinate> GetCoordinates(Address address)
        {
            if (address == null || !(address.HasFullAddress || address.HasCityAndState))
                return new ErrorDictionary(nameof(address), "Address info not supplied.");

            if ((MaaSConfig.Current.Environment.GoogleServices.MapsApiKeyUnrestricted ?? MaaSConfig.Current.Environment.GoogleServices.MapsApiKey) == null)
                return new ErrorDictionary("", "Google Maps API key not supplied.");

            var addressList = AddressHelper.GetAddressesForGeocoding(address);

            foreach (var addr in addressList)
            {
                var coords = GoogleGeocoder.Geocode(addr);
                if (coords != null) return coords;
            }

            var fullAddresss = addressList.First();
            return new ErrorDictionary("", $"No results for address '{fullAddresss}'.");
        }
    }
}
