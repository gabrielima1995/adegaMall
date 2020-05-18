using Application.Config;
using Framework.Geography;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Infrastructure.Geocoding
{
    internal static class GoogleGeocoder
    {
        public static GeoCoordinate Geocode(string address)
        {
            var request = WebRequest.Create(string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false&key={1}",
                Uri.EscapeDataString(address), MaaSConfig.Current.Environment.GoogleServices.MapsApiKeyUnrestricted));

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = reader.ReadToEnd();
                    var json = JsonConvert.DeserializeAnonymousType(responseBody, new
                    {
                        status = "",
                        error_message = "",
                        results = new[]
                        {
                            new
                            {
                                geometry = new
                                {
                                    location = new { lat = 0D, lng = 0D }
                                }
                            }
                        }
                    });

                    if (json.status == "OK" && json.results.Any())
                    {
                        var location = json.results.First().geometry.location;
                        return new GeoCoordinate(location.lat, location.lng);
                    }
                    else if (json.status == "REQUEST_DENIED")
                    {
                        throw new GeocodingRequestDeniedException(json.error_message);
                    }
                }
            }

            return null;
        }
    }
}
