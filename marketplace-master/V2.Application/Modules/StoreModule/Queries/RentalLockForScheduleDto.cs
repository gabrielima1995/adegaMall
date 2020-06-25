using Domain.StoreModule.Entities;
using Framework;
using System;

namespace V2.Application.Modules.StoreModule.Queries
{
    public class RentalLockForScheduleDto
    {
        private MarketplaceTimeZone _timeZone;

        public Guid Id { get; set; }

        public LockType Type { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public Guid? ProductId { get; set; }
        public string ProductTitle { get; set; }

        public string TimeZoneId { get; set; }

        public MarketplaceTimeZone TimeZone => _timeZone ?? (_timeZone = TimeZones.Find(TimeZoneId));

        public DateTime? StartDateTime => StartDate?.Add(StartTime.Value);

        public DateTime? EndDateTime => EndDate?.Add(EndTime.Value);

        public LockMode Mode { get; set; }

        public DateTime? StartDateTimeUTC => StartDateTime.HasValue
            ? TimeZone.ConvertTimeToUtc(StartDateTime.Value)
            : default(DateTime?);

        public DateTime? EndDateTimeUTC => EndDateTime.HasValue
            ? TimeZone.ConvertTimeToUtc(EndDateTime.Value)
            : default(DateTime?);
    }
}