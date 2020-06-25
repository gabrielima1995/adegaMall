using Infra.Crosscutting.Globalization;
using NewContext.Queries.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Areas.Admin.Models
{
    public class StringsViewModel
    {
        public string[] Languages { get; set; }

        public IEnumerable<TranslationViewModel> Translations { get; set; }

        public static StringsViewModel From(StringsForAdminTranslationDto dto)
        {
            return new StringsViewModel
            {
                Languages = dto.Languages.ToArray(),
                Translations = dto.LabelsWithTranslations.Select(item => new TranslationViewModel
                {
                    Label = item.Label,
                    Strs = item.Translations.Select(t => new StrViewModel
                    {
                        Lang = LangHelper.GetLangFromCultureSpecifier(t.Language),
                        Value = t.Value
                    }).ToList()
                })
            };
        }
    }

    public class TranslationViewModel
    {
        public string Label { get; set; }
        public IList<StrViewModel> Strs { get; set; }
    }
}