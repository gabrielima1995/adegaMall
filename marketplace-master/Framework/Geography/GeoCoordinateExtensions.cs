using System;

namespace Framework.Geography
{
    public static class GeoCoordinateExtensions
    {
        public static double DistanceTo(this GeoCoordinate baseCoordinates, GeoCoordinate targetCoordinates)
        {
            return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometers);
        }

        public static double DistanceTo(this GeoCoordinate baseCoordinates, GeoCoordinate targetCoordinates, UnitOfLength unitOfLength)
        {
            var baseRad = Math.PI * baseCoordinates.Lat / 180;
            var targetRad = Math.PI * targetCoordinates.Lat / 180;
            var theta = baseCoordinates.Lng - targetCoordinates.Lng;
            var thetaRad = Math.PI * theta / 180;

            var dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) +
                Math.Cos(baseRad) * Math.Cos(targetRad) * Math.Cos(thetaRad);

            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }
    }
}