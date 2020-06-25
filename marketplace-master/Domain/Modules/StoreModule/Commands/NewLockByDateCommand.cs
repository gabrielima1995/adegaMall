using Domain.Core;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class NewLockByDateCommand : Command
    {
        public Guid LockId { get; set; }

        public string SellerId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public Guid? ProductId { get; set; }

        public string TimeZoneId { get; set; }

        public LockMode Mode { get; set; }

        public NewLockByDateCommand(Guid lockId, Seller seller,
            DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime,
            Product product, string timeZoneId, LockMode mode)
            : base(lockId.ToString())
        {
            Guard.Ensure.NotEmpty(lockId, nameof(lockId));
            Guard.Ensure.NotNull(seller, nameof(seller));
            Guard.Ensure.NotNull(startDate, nameof(startDate));
            Guard.Ensure.NotNull(endDate, nameof(endDate));
            Guard.Ensure.NotNull(startTime, nameof(startTime));
            Guard.Ensure.NotNull(endTime, nameof(endTime));
            Guard.Ensure.Rule(() => startDate.Date <= endDate.Date, nameof(startDate), "Start date must be greater than end date");

            LockId = lockId;
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
            SellerId = seller.Id;

            ProductId = product?.Id;

            TimeZoneId = timeZoneId;

            Mode = mode;
        }
    }
}