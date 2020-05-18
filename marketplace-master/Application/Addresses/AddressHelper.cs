using Geocoding;
using System;
using System.Linq;

namespace Application.Addresses
{
    public static class AddressHelper
    {
        private enum AddressPrecision
        {
            StreetAndNumber,
            Street,
            City
        }

        public static void UpdateLocation(this IGeocoder geocoder, Domain.Entities.Address address)
        {
            if (address != null && (address.HasFullAddress || address.HasCityAndState))
            {
                var precisionModes = address.HasFullAddress
                    ? new[] { AddressPrecision.StreetAndNumber, AddressPrecision.Street, AddressPrecision.City }
                    : new[] { AddressPrecision.City };

                foreach (var p in precisionModes)
                {
                    var addressString = address.GetAddressForGeocoding(p);
                    var list = geocoder.Geocode(addressString).ToList();
                    if (!list.Any()) continue;

                    var result = list.First();
                    address.SetLocation(result.Coordinates.Latitude, result.Coordinates.Longitude);
                    return;
                }
            }

            address.SetLocation(0, 0);
        }

        private static string GetAddressForGeocoding(this Domain.Entities.Address address, AddressPrecision precision)
        {
            if (address.HasFullAddress)
            {
                string addr = "";

                switch (precision)
                {
                    case AddressPrecision.StreetAndNumber:
                        addr = $"{address.Street}, {address.Number}, {address.CityAndState}, {address.ZipCode}, Brasil";
                        break;
                    case AddressPrecision.Street:
                        addr = $"{address.Street}, {address.CityAndState}, {address.ZipCode}, Brasil";
                        break;
                    case AddressPrecision.City:
                        addr = $"{address.CityAndState}, {address.ZipCode}, Brasil";
                        break;
                    default:
                        throw new InvalidOperationException($"Application.Addresses.AddressHelper.GetAddressForGeocoding: precision {precision} is not supported.");
                }

                return addr.ReplaceAccents().Trim();
            }

            if (address.HasCityAndState)
            {
                var addr = $"{address.CityAndState}, Brasil".ReplaceAccents();
                return addr.Trim();
            }

            return "";
        }
    }
}