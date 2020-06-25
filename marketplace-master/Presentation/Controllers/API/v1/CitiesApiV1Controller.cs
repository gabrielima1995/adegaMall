using DevTrends.MvcDonutCaching;
using Domain.Entities;
using Framework;
using MoreLinq;
using System.Linq;
using System.Web.Http;

namespace Presentation.Controllers.API.v1
{
    [AllowAnonymous]
    [RoutePrefix("api/v1/cities")]
    [DonutOutputCache(CacheProfile = "General")]
    public class CitiesApiV1Controller : ApiController
    {
        private readonly IRepository<City> _cityRepository;

        public CitiesApiV1Controller(
            IRepository<City> cityRepository)
        {
            _cityRepository = cityRepository;
        }

        [Route("search", Name = "Cities search")]
        [HttpGet]
        public IHttpActionResult Search(string query = "")
        {
            var cities = _cityRepository
                .Query()
                .Where(i => i.Name.Contains(query))
                .OrderBy(i => i.Name.StartsWith(query) ? -1 : 1)
                .ThenBy(i => System.Math.Abs(i.Name.Length - query.Length))
                .ThenBy(i => i.Name)
                .Take(20)
                .Select(i => new
                {
                    id = i.Name + " - " + i.State,
                    text = i.Name + " - " + i.State,
                    lat = (i.Location != null ? i.Location.Latitude : 0),
                    lng = (i.Location != null ? i.Location.Longitude : 0)
                })
                .ToList().DistinctBy(x => x.id).ToList();

            return Ok(cities);
        }
    }
}
