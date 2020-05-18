using System;

namespace Infrastructure.Geocoding
{
    public class GeocodingRequestDeniedException : Exception
    {
        public GeocodingRequestDeniedException(string errorMessage) : base("Request denied: " + errorMessage)
        {

        }
    }
}
