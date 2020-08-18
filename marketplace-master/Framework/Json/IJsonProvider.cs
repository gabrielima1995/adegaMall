using Newtonsoft.Json;
using System.IO;

namespace Framework.Json
{
    public interface IJsonProvider
    {
        T FromJson<T>(string json, JsonSerializerSettings settings = null);

        string ToJson<T>(T data, JsonSerializerSettings settings = null);

        T FromStream<T>(Stream s, JsonSerializerSettings settings = null);

        Stream ToStream<T>(T data, JsonSerializerSettings settings = null);
    }
}