using System.Collections.Generic;

namespace NewContext.Queries.Dtos
{
    public class LanguageForConfigurationDto
    {
        public string Language { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }

    public class LocalizedStringDto
    {
        public string Language { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class StringsForAdminTranslationDto
    {
        public IReadOnlyList<string> Languages { get; set; }
        public IReadOnlyList<LabelWithTranslations> LabelsWithTranslations { get; set; }

        public class LabelWithTranslations
        {
            public string Label { get; set; }
            public IReadOnlyList<Translation> Translations { get; set; }
        }

        public class Translation
        {
            public string Language { get; set; }
            public string Value { get; set; }
        }
    }
}
