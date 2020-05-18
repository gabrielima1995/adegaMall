namespace Framework.Geography
{
    public class UnitOfLength
    {
        public static UnitOfLength Kilometers { get; } = new UnitOfLength(1.609344);
        public static UnitOfLength NauticalMiles { get; } = new UnitOfLength(0.8684);
        public static UnitOfLength Miles { get; } = new UnitOfLength(1);

        private readonly double _fromMilesFactor;

        private UnitOfLength(double fromMilesFactor)
        {
            _fromMilesFactor = fromMilesFactor;
        }

        public double ConvertFromMiles(double input)
        {
            return input * _fromMilesFactor;
        }
    }
}