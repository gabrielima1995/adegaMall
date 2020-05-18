using Infra.Crosscutting.Globalization;

namespace Infra.BatchImportAdapter.Internals
{
    internal static class Localization
    {
        public static string Get(string label) => GlobalizationHelper.GetString(label);
    }
}
