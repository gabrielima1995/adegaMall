using Infra.Crosscutting.Globalization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Domain.StoreModule.Entities
{
    public enum RentalPeriodUnit
    {
        [Display(Name = "Hora")]
        [DescriptionLocalized("RENTAL_TIMESPAN_HOUR")]
        Hour = 0,

        [Display(Name = "Dia")]
        [DescriptionLocalized("RENTAL_TIMESPAN_DAY")]
        Day = 1,

        [Display(Name = "Mês")]
        [DescriptionLocalized("RENTAL_TIMESPAN_MONTH")]
        Month = 2,

        [Display(Name = "Minuto")]
        [DescriptionLocalized("RENTAL_TIMESPAN_MINUTE")]
        Minute = 3
    }
}
