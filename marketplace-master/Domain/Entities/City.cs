using Framework.Helpers;
using System.Data.Entity.Spatial;

namespace Domain.Entities
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public DbGeography Location { get; private set; }

        public bool HasLocation =>
            Location != null;

        public void SetLocation(double lat, double lng)
        {
            Location = DbGeographyHelper.FromLocation(lat, lng);
        }
    }
}
