using System.ComponentModel.DataAnnotations;
using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.Modules.StoreModule.Entities
{
    public enum RecurringProductAvailabilityUnit
    {
        [DescriptionLocalized("RENTAL_TIMESPAN_DAY")]
        Days = 0,

        [DescriptionLocalized("RENTAL_TIMESPAN_WEEK")]
        Weeks = 1
    }
}