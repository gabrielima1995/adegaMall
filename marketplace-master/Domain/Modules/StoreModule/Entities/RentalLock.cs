using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Commands;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.StoreModule.Entities
{
    public class RentalLock : SoftDeletableEntity<Guid>
    {
        private TimeSpan minTime = new TimeSpan(0, 0, 0);
        private TimeSpan maxTime = new TimeSpan(23, 59, 0);

        public LockType Type { get; private set; }

        public DateTime? StartDate { get; private set; }

        public DateTime? EndDate { get; private set; }

        public DayOfWeek? DayOfWeek { get; private set; }

        public TimeSpan? StartTime { get; private set; }

        public TimeSpan? EndTime { get; private set; }

        [ForeignKey("Seller")]
        public string Seller_Id { get; private set; }

        public Seller Seller { get; private set; }

        [ForeignKey("Product")]
        public Guid? Product_Id { get; private set; }

        public Product Product { get; private set; }

        public string TimeZoneId { get; private set; }

        public LockMode Mode { get; private set; }

        public bool IsStartOfDay => StartTime == minTime;

        public bool IsEndOfDay => EndTime == maxTime;

        public static RentalLock New(NewLockByDateCommand command)
        {
            var item = new RentalLock
            {
                Id = command.LockId,
                Type = LockType.ByDate,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                Seller_Id = command.SellerId,
                Product_Id = command.ProductId,
                TimeZoneId = command.TimeZoneId,
                Mode = command.Mode
            };

            item.AddDomainEvent(new RentalLockAdded(item));

            return item;
        }

        public static RentalLock New(NewLockByDayOfWeekCommand command)
        {
            var item = new RentalLock
            {
                Id = command.LockId,
                Type = LockType.ByDayOfWeek,
                DayOfWeek = command.DayOfWeek,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                Seller_Id = command.SellerId,
                Product_Id = command.ProductId,
                TimeZoneId = command.TimeZoneId,
                Mode = command.Mode
            };

            item.AddDomainEvent(new RentalLockAdded(item));

            return item;
        }

        public RentalLock Delete()
        {
            Deleted = true;
            DeletedAt = DateTime.UtcNow;

            AddDomainEvent(new RentalLockDeleted(this));

            return this;
        }
    }
}