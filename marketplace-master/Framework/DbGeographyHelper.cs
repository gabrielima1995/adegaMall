using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Framework.Geography
{
    public static class DbGeographyHelper
    {
        public static DbGeography AsDbGeography(this GeoCoordinate coordinate)
        {
            return DbGeography.FromText(coordinate.ToWellKnownText(), 4326);
        }

        public static DbGeography AsDbGeography(this GeoBounds bounds)
        {
            if (bounds == null)
                throw new ArgumentException("Valid Origin and End coordinates must be provided in order to create a Polygon.", nameof(bounds));

            double lat1 = bounds.Origin.Lat, lat2 = bounds.End.Lat;
            double lng1 = bounds.Origin.Lng, lng2 = bounds.End.Lng;

            var tl = new GeoCoordinate(
                Math.Max(lat1, lat2),
                Math.Min(lng1, lng2));

            var tr = new GeoCoordinate(
                Math.Max(lat1, lat2),
                Math.Max(lng1, lng2));

            var br = new GeoCoordinate(
                Math.Min(lat1, lat2),
                Math.Max(lng1, lng2));

            var bl = new GeoCoordinate(
                Math.Min(lat1, lat2),
                Math.Min(lng1, lng2));

            return CreatePolygon(new GeoCoordinate[] { tl, tr, br, bl, tl });
        }

        public static DbGeography CreatePolygon(IEnumerable<GeoCoordinate> coordinates)
        {
            var coordinateList = coordinates.ToList();

            if (coordinateList.First() != coordinateList.Last())
                throw new Exception("First and last point do not match. This is not a valid polygon.");

            var count = 0;
            var sb = new StringBuilder();
            var culture = CultureInfo.InvariantCulture;

            sb.Append(@"POLYGON((");

            foreach (var coordinate in coordinateList)
            {
                if (count == 0)
                    sb.Append(coordinate.Lng.ToString(culture) + " " + coordinate.Lat.ToString(culture));
                else
                    sb.Append("," + coordinate.Lng.ToString(culture) + " " + coordinate.Lat.ToString(culture));

                count++;
            }

            sb.Append(@"))");

            var poly = FixPolygon(DbGeography.PolygonFromText(sb.ToString(), 4326));

            return poly;
        }

        private static DbGeography FixPolygon(DbGeography polygon)
        {
            if (polygon.Area.HasValue && polygon.Area.Value > 255000000000000L)
            {
                var sqlPolygon = SqlGeography.STGeomFromWKB(new System.Data.SqlTypes.SqlBytes(polygon.AsBinary()), DbGeography.DefaultCoordinateSystemId);
                sqlPolygon = sqlPolygon.ReorientObject();
                polygon = DbGeography.FromBinary(sqlPolygon.STAsBinary().Value);
            }

            return polygon;
        }
    }
}
