using System.Text;

namespace Application.Config
{
    public static class MaaSConfigHelper
    {
        public static string CreateSassFileContent(this MaaSConfig config)
        {
            var variables = config.Layout.CreateSassVariables();

            var sb = new StringBuilder();

            foreach (var variable in variables)
            {
                sb.AppendLine($"{variable.Key}: {variable.Value}");
            }

            return sb.ToString();
        }
    }
}
