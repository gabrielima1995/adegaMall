using System;
using System.IO;
using System.Reflection;

namespace Domain.Data.SqlScripts
{
    internal static class SqlScriptHelper
    {
        public static string GetScript(ScriptName script)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var scriptName = Enum.GetName(typeof(ScriptName), script);
            var resourceName = "Infrastructure.Data.SqlScripts." + scriptName + ".sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
