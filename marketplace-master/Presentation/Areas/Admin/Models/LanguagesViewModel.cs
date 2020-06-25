using NewContext.Queries.Dtos;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Models
{
    public class LanguagesViewModel
    {
        public LanguagesViewModel()
        {
        }

        public LanguagesViewModel(IEnumerable<LanguageForConfigurationDto> languagesForConfiguration)
        {
            AllLanguages = languagesForConfiguration.Select(dto => LanguageItem.From(dto));
            ActiveLanguages = AllLanguages.Where(lang => lang.IsActive).ToArray();
        }

        public IEnumerable<LanguageItem> AllLanguages { get; set; }

        public IEnumerable<LanguageItem> ActiveLanguages { get; set; }

        public SelectList ActivationList => new SelectList(AllLanguages.Where(lang => !lang.IsActive), "Language", "DisplayName");

        public class LanguageItem
        {
            public string Language { get; set; }
            public bool IsActive { get; set; }
            public bool IsDefault { get; set; }
            public string DisplayName => $"{Language}: {GetCultureDisplayName()}";

            public static LanguageItem From(LanguageForConfigurationDto dto)
            {
                return new LanguageItem
                {
                    Language = dto.Language,
                    IsActive = dto.IsActive,
                    IsDefault = dto.IsDefault
                };
            }

            private string GetCultureDisplayName() => CultureInfo.CreateSpecificCulture(Language).DisplayName;
        }
    }
}