using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewAvailabilityDayOfWeekAppCommand
    {
        public DayOfWeek DayOfWeek { get; private set; }

        public TimeSpan? StartTime1 { get; private set; }

        public TimeSpan? EndTime1 { get; private set; }

        public bool Time1Setted => StartTime1.HasValue && EndTime1.HasValue;

        public TimeSpan? StartTime2 { get; private set; }

        public TimeSpan? EndTime2 { get; private set; }

        public bool Time2Setted => StartTime2.HasValue && EndTime2.HasValue;

        protected NewAvailabilityDayOfWeekAppCommand()
        {
        }

        public static NewAvailabilityDayOfWeekAppCommand New(DayOfWeek dayOfWeek,
            TimeSpan? startTime1, TimeSpan? endTime1,
            TimeSpan? startTime2, TimeSpan? endTime2)
        {
            return new NewAvailabilityDayOfWeekAppCommand
            {
                DayOfWeek = dayOfWeek,
                StartTime1 = startTime1,
                EndTime1 = endTime1,
                StartTime2 = startTime2,
                EndTime2 = endTime2
            };
        }

        public void UpdateTimes(TimeSpan? startTime1, TimeSpan? endTime1,
            TimeSpan? startTime2, TimeSpan? endTime2)
        {
            StartTime1 = startTime1;
            EndTime1 = endTime1;
            StartTime2 = startTime2;
            EndTime2 = endTime2;
        }
    }
}