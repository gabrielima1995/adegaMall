using Domain.StoreModule.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class NewLockVM
    {
        [DisplayNameLocalized("NEW_LOCK_TYPE_LABEL")]
        public LockType Type { get; set; }

        [DisplayNameLocalized("NEW_LOCK_START_DATE_LABEL")]
        [DataType(DataType.Date)]
        [RequiredIfLocalized("Type == Domain.StoreModule.Entities.LockType.ByDate")]
        public DateTime? StartDate { get; set; }

        [DisplayNameLocalized("NEW_LOCK_END_DATE_LABEL")]
        [DataType(DataType.Date)]
        [RequiredIfLocalized("Type == Domain.StoreModule.Entities.LockType.ByDate")]
        public DateTime? EndDate { get; set; }

        [DisplayNameLocalized("NEW_LOCK_DAY_OF_WEEK_LABEL")]
        [RequiredIfLocalized("Type == Domain.StoreModule.Entities.LockType.ByDayOfWeek")]
        public DayOfWeek? DayOfWeek { get; set; }

        [DisplayNameLocalized("NEW_LOCK_START_TIME_LABEL")]
        [DataType(DataType.Time)]
        [RequiredLocalized]
        public TimeSpan? StartTime { get; set; }

        [DisplayNameLocalized("NEW_LOCK_END_TIME_LABEL")]
        [DataType(DataType.Time)]
        [RequiredLocalized]
        public TimeSpan? EndTime { get; set; }

        public bool IsSpecificProduct { get; set; }

        [DisplayNameLocalized("NEW_LOCK_PRODUCT_LABEL")]
        public string ProductId { get; set; }

        [DisplayNameLocalized("NEW_LOCK_MODE_LABEL")]
        public LockMode Mode { get; set; } = LockMode.FullyUnavailable;
    }
}