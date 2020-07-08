using Framework.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;

namespace V2.Infrastructure.Json
{
    public class JsonProvider : IJsonProvider
    {
        public static readonly Lazy<JsonSerializerSettings> JsonSettingsLazy =
            new Lazy<JsonSerializerSettings>(() =>
            {
                var contractResolver = new CamelCasePropertyNamesContractResolver();

                return new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    DateFormatString = @"yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = contractResolver,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            });

        public T FromJson<T>(string json, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(json, settings ?? JsonSettingsLazy.Value);
        }

        public string ToJson<T>(T data, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(data, settings ?? JsonSettingsLazy.Value);
        }

        public T FromStream<T>(Stream s, JsonSerializerSettings settings = null)
        {
            s.Seek(0, SeekOrigin.Begin);

            using (var sr = new StreamReader(s, Encoding.UTF8, true, 1024, true))
            {
                var serializer = JsonSerializer.Create(settings ?? JsonSettingsLazy.Value);
                return (T)serializer.Deserialize(sr, typeof(T));
            }
        }

        public Stream ToStream<T>(T data, JsonSerializerSettings settings = null)
        {
            var ms = new MemoryStream();

            using (var sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
            {
                var serializer = JsonSerializer.Create(settings ?? JsonSettingsLazy.Value);
                serializer.Serialize(sw, data);

                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                return sw.BaseStream;
            }
        }
    }
}