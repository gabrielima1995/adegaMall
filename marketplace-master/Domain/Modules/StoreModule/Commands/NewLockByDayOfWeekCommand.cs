using Domain.Core;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class NewLockByDayOfWeekCommand : Command
    {
        public Guid LockId { get; set; }

        public string SellerId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; }

        public TimeSpan EndTime { get; }

        public Guid? ProductId { get; set; }

        public string TimeZoneId { get; set; }

        public LockMode Mode { get; set; }

        public NewLockByDayOfWeekCommand(Guid lockId, Seller seller,
            DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime,
            Product product, string timeZoneId, LockMode mode)
            : base(lockId.ToString())
        {
            Guard.Ensure.NotEmpty(lockId, nameof(lockId));
            Guard.Ensure.NotNull(seller, nameof(seller));

            LockId = lockId;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;

            SellerId = seller.Id;
            ProductId = product?.Id;

            TimeZoneId = timeZoneId;

            Mode = mode;
        }
    }
}