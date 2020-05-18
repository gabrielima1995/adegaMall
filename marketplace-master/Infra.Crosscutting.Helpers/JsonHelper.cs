using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace System
{
    public static class JsonHelper
    {
        private static JsonSerializerSettings GetSettingsDefault()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public static string ToJson<T>(this T value, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(value, settings ?? GetSettingsDefault());
        }

        public static T FromJson<T>(this string value, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(value, settings ?? GetSettingsDefault());
        }
    }
}