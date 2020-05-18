using System;

namespace Framework.Geography
{
    public class GeoBounds
    {
        public GeoCoordinate Origin { get; private set; }

        public GeoCoordinate End { get; private set; }

        public GeoBounds(GeoCoordinate origin, GeoCoordinate end)
        {
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
            End = end ?? throw new ArgumentNullException(nameof(end));
        }

        public GeoCoordinate GetCenter()
        {
            return new GeoCoordinate(
                (Origin.Lat + End.Lat) / 2,
                (Origin.Lng + End.Lng) / 2);
        }
    }
}
