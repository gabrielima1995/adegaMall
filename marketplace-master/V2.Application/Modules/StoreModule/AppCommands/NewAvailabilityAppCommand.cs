using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewAvailabilityAppCommand
    {
        public string SellerId { get; private set; }

        public List<NewAvailabilityDayOfWeekAppCommand> DaysOfWeek { get; private set; } =
            new List<NewAvailabilityDayOfWeekAppCommand>(7);

        public MarketplaceTimeZone TimeZone { get; private set; }

        public Guid? ProductId { get; private set; }

        protected NewAvailabilityAppCommand()
        {
        }

        public static NewAvailabilityAppCommand New(string sellerId, MarketplaceTimeZone timeZone, Guid? productId)
        {
            return new NewAvailabilityAppCommand
            {
                SellerId = sellerId,
                TimeZone = timeZone,
                ProductId = productId
            };
        }

        public NewAvailabilityDayOfWeekAppCommand AddDayOfWeek(DayOfWeek dayOfWeek,
            TimeSpan? startTime1, TimeSpan? endTime1,
            TimeSpan? startTime2, TimeSpan? endTime2)
        {
            var item = DaysOfWeek.FirstOrDefault(i => i.DayOfWeek == dayOfWeek);

            if (item == null)
            {
                item = NewAvailabilityDayOfWeekAppCommand.New(dayOfWeek, startTime1, endTime1, startTime2, endTime2);
            }
            else
            {
                item.UpdateTimes(startTime1, endTime1, startTime2, endTime2);
            }

            DaysOfWeek.Add(item);

            return item;
        }
    }
}