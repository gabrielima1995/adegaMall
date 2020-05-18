using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PaymentIntegrator.Moip.Utils
{
    internal static class JsonHelper
    {
        internal static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, GetSettings());
        }

        internal static object Deserialize(string json)
        {
            return JsonConvert.DeserializeObject(json, GetSettings());
        }

        internal static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, GetSettings());
        }

        private static JsonSerializerSettings GetSettings() => new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new ContractResolverWithPrivates()
        };

        private class ContractResolverWithPrivates : CamelCasePropertyNamesContractResolver
        {
            protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as System.Reflection.PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }
    }
}
