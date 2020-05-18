using System.IO;
using System.Reflection;

namespace V2.Infrastructure.Data.SqlScripts
{
    internal static class SqlScriptHelper
    {
        public static string GetScript(string scriptName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "V2.Infrastructure.Data.SqlScripts." + scriptName + ".sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
