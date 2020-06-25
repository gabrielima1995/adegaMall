namespace Newtonsoft.Json.Serialization
{
    /// <summary>
    /// Resolves member mappings for a type, snake casing property names.
    /// </summary>
    public class SnakeCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public SnakeCasePropertyNamesContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
        }
    }
}