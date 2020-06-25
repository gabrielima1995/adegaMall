using System.Data.Entity.Spatial;

namespace Domain.DTO
{
    public class AddressListing
    {
        public string Street { get; set; }

        public string Number { get; set; }

        public string AdditionalInfo { get; set; }

        public string Neighborhood { get; set; }

        public string ZipCode { get; set; }

        public string CityState { get; set; }

        public DbGeography Location { get; set; }
    }
}