using Domain.StoreModule.Entities;
using Framework;
using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewLockAppCommand
    {
        public string SellerId { get; set; }

        public LockType Type { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public MarketplaceTimeZone TimeZone { get; set; }

        public Guid? ProductId { get; set; }

        public LockMode Mode { get; set; }

        public static NewLockAppCommand NewLockByDayOfWeek(string sellerId, MarketplaceTimeZone timeZone,
            DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime,
            LockMode mode, Guid? productId)
        {
            return new NewLockAppCommand
            {
                SellerId = sellerId,
                Type = LockType.ByDayOfWeek,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                TimeZone = timeZone,
                ProductId = productId,
                Mode = mode
            };
        }
    }
}
