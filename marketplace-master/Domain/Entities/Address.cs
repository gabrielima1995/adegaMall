using Framework.Helpers;
using PaymentIntegrator.Core.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace Domain.Entities
{
    public class Address : BaseEntity<Guid>, IAddress
    {
        private string _cityAndState;

        public string Street { get; set; }

        public string Number { get; set; }

        public string AdditionalInfo { get; set; }

        public string Neighborhood { get; set; }

        public string ZipCode { get; set; }

        public string CityAndState
        {
            get { return _cityAndState; }
            set
            {
                _cityAndState = value;

                if (!string.IsNullOrWhiteSpace(_cityAndState))
                {
                    string[] split = _cityAndState.IndexOf('-') > 0
                       ? _cityAndState.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
                       : _cityAndState.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length > 1)
                        State = split[split.Length - 1].Trim();

                    if (split.Length > 0)
                        City = string.Join(" ", split.Take(split.Length - 1)).Trim();
                }
            }
        }

        public DbGeography Location { get; private set; }

        public bool HasCityAndState => !string.IsNullOrWhiteSpace(CityAndState);

        public bool HasZipCode => !string.IsNullOrEmpty(ZipCode);

        public bool HasFullAddress
        {
            get
            {
                return HasCityAndState &&
                    !string.IsNullOrWhiteSpace(Street) &&
                    !string.IsNullOrWhiteSpace(Number) &&
                    !string.IsNullOrWhiteSpace(Neighborhood);
            }
        }

        public bool HasLocation => Location != null;

        public string StreetNumber => Number;

        public string Complementary => AdditionalInfo;

        [NotMapped]
        public string City { get; private set; }

        [NotMapped]
        public string State { get; private set; }

        public Address()
        {
            Id = Guid.NewGuid();
        }

        public void SetLocation(double lat, double lng)
        {
            Location = DbGeographyHelper.FromLocation(lat, lng);
        }

        public override string ToString()
        {
            if (HasFullAddress)
            {
                var additionalInfo = !string.IsNullOrWhiteSpace(AdditionalInfo) ? $", {AdditionalInfo}" : "";
                return $"{Street}, {Number} {additionalInfo} - {Neighborhood}, {CityAndState} - {ZipCode}";
            }

            if (HasCityAndState)
                return $"{CityAndState} - {ZipCode}";

            return $"{ZipCode}";
        }
    }
}