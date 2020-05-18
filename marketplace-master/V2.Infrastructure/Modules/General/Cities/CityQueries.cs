using Dapper;
using Domain.Interfaces;
using System;
using System.Data.Entity.Spatial;
using V2.Application.Cities;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.General.Cities
{
    public class CityQueries : ICityQueries
    {
        private readonly ICache _cache;

        public CityQueries(ICache cache)
        {
            _cache = cache;

            using (var dbCon = MyDbConnection.Create())
            {
                var results = dbCon.Query("SELECT * FROM dbo.City");
                foreach (var r in results)
                {
                    string cityAndState = r.Name + " - " + r.State;
                    if (!cache.Contains(cityAndState))
                    {
                        var geography = DbGeography.FromText(r.Location.ToString());

                        cache.Add(cityAndState, geography);
                    }
                }
            }
        }

        public DbGeography GetCityDbGeography(string cityAndState)
        {
            var result = _cache.Get<DbGeography>(cityAndState);
            return result;
        }

        public Tuple<double, double> GetCityLatLng(string cityAndState)
        {
            var result = GetCityDbGeography(cityAndState);

            return result?.Latitude != null && result.Longitude != null
                ? new Tuple<double, double>(result.Latitude.Value, result.Longitude.Value)
                : null;
        }
    }
}