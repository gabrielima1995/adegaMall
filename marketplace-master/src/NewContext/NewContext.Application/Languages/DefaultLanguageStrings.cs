using CSharpFunctionalExtensions;
using NewContext.Domain.Languages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    internal static class DefaultLanguageStrings
    {
        private static IReadOnlyList<LocalizedString> localizedStrings;

        public static async Task<Maybe<LocalizedString>> GetAsync(Label label)
        {
            var list = await ListAllAsync();
            return list.FirstOrDefault(item => item.Label == label);
        }

        public static async Task<IEnumerable<LocalizedString>> ListAllAsync()
        {
            if (localizedStrings == null)
            {
                var lines = await LoadStringsAsync();
                localizedStrings = ParseLocalizedStrings(lines);
            }

            return localizedStrings;
        }

        private static async Task<IReadOnlyList<string>> LoadStringsAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "NewContext.Application.Languages.DefaultLanguageStrings.tsv";
            var list = new List<string>();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                do
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) continue;
                    list.Add(line);
                } while (!reader.EndOfStream);
            }

            return list;
        }

        private static IReadOnlyList<LocalizedString> ParseLocalizedStrings(IEnumerable<string> lines)
        {
            var list = new List<LocalizedString>(lines.Count());

            foreach (var line in lines)
            {
                var parts = line.Split('\t');

                if (parts.Length != 2)
                    continue;

                var label = parts[0];
                var value = parts[1];

                Label.Create(label)
                    .OnSuccess(labelResult => LocalizedString.Create(ApplicationLanguages.Default, labelResult, value))
                    .OnSuccess(localizedStringResult => list.Add(localizedStringResult));
            }

            return list.AsReadOnly();
        }
    }
}
