using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Infrastructure.Geocoding
{
    internal static class AddressHelper
    {
        public static IEnumerable<string> GetAddressesForGeocoding(Address address)
        {
            var precisionModes = address.HasFullAddress
                ? new[] { AddressPrecision.StreetAndNumber, AddressPrecision.Street, AddressPrecision.City }
                : new[] { AddressPrecision.City };

            var list = new string[precisionModes.Length];

            for (int i = 0; i < precisionModes.Length; i++)
                list[i] = GetAddressForGeocoding(address, precisionModes[i]);

            return list;
        }

        private static string GetAddressForGeocoding(Address address, AddressPrecision precision)
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
                    throw new InvalidOperationException($"Infraestructure.Geocoding.GeocodingService.GetAddressForGeocoding: precision {precision} is not supported.");
            }

            return addr.ReplaceAccents().Trim();
        }

        private enum AddressPrecision
        {
            StreetAndNumber,
            Street,
            City
        }
    }
}
