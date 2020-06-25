using System;

namespace Newtonsoft.Json.Converters
{
    public class CustomIsoDateTimeConverter : IsoDateTimeConverter
    {
        public CustomIsoDateTimeConverter()
        {
            base.DateTimeFormat = DateTimeHelper.ISO_FORMAT;
        }
    }
}