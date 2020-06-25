using Domain.Core;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.StoreModule.Entities
{
    public class RentalShiftTime : ValueObject, IComparable<RentalShiftTime>
    {
        private const string errorMessage = "The time must range from 00:00 to 23:59.";

        private RentalShiftTime()
        {
        }

        private RentalShiftTime(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }

        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        public static OperationResult<RentalShiftTime> Create(int hours, int minutes)
        {
            var errors = new ErrorDictionary();

            if (hours < 0 || hours > 23)
                errors.Add(nameof(hours), errorMessage);

            if (minutes < 0 || minutes > 59)
                errors.Add(nameof(minutes), errorMessage);

            if (errors.HasErrors)
                return new OperationResult<RentalShiftTime>(errors);

            return new OperationResult<RentalShiftTime>(new RentalShiftTime(hours, minutes));
        }

        public int CompareTo(RentalShiftTime other)
        {
            if (Hours < other.Hours) return -1;
            if (Hours == other.Hours && Minutes < other.Minutes) return -1;
            if (Hours == other.Hours && Minutes == other.Minutes) return 0;
            return 1;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Hours;
            yield return Minutes;
        }

        internal void UpdateValues(RentalShiftTime time)
        {
            Hours = time.Hours;
            Minutes = time.Minutes;
        }

        public static bool operator >(RentalShiftTime a, RentalShiftTime b) => a.CompareTo(b) == 1;
        public static bool operator <(RentalShiftTime a, RentalShiftTime b) => a.CompareTo(b) == -1;
    }
}
