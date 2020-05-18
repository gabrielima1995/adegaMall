using System;
using System.Data.Entity.Spatial;

namespace V2.Application.Cities
{
    public interface ICityQueries
    {
        DbGeography GetCityDbGeography(string cityAndState);

        Tuple<double, double> GetCityLatLng(string cityAndState);
    }
}
