using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Presentation.Models.ScheduleViewModels
{
    public class NewAvailabilityVM
    {
        [RequiredLocalized]
        public List<NewAvailabilityDayOfWeekVM> DaysOfWeek { get; set; } =
            new List<NewAvailabilityDayOfWeekVM>(7);

        [DisplayNameLocalized("NEW_LOCK_PRODUCT_LABEL")]
        public Guid? ProductId { get; set; }

        public bool IsSpecificProduct => ProductId.HasValue;

        public static NewAvailabilityVM Prepare(Guid? productId, List<RentalLock> locks)
        {
            var model = new NewAvailabilityVM
            {
                ProductId = productId
            };

            void AddDayOfWeek(DayOfWeek dayOfWeek)
            {
                var dayOfWeekLocks = locks
                    .Where(i => i.DayOfWeek == dayOfWeek)
                    .ToList();

                model.DaysOfWeek.Add(new NewAvailabilityDayOfWeekVM(dayOfWeek, dayOfWeekLocks));
            }

            AddDayOfWeek(DayOfWeek.Sunday);
            AddDayOfWeek(DayOfWeek.Monday);
            AddDayOfWeek(DayOfWeek.Tuesday);
            AddDayOfWeek(DayOfWeek.Wednesday);
            AddDayOfWeek(DayOfWeek.Thursday);
            AddDayOfWeek(DayOfWeek.Friday);
            AddDayOfWeek(DayOfWeek.Saturday);

            return model;
        }
    }
}