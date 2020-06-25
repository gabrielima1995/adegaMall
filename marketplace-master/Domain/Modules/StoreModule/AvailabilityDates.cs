using Framework;
using Infra.Crosscutting.Globalization;
using System;

namespace Domain.Services.StoreModule
{
    public struct AvailabilityDates
    {
        public DateTime Start { get; }

        public DateTime StartUTC { get; }

        public DateTime End { get; }

        public DateTime EndUTC { get; }

        public MarketplaceTimeZone TimeZone { get; }

        public bool IsAvailable { get; }

        public int UnitsAvailable { get; }

        private AvailabilityDates(DateTime startUTC, DateTime endUTC, MarketplaceTimeZone timeZone,
            bool isAvailable, int unitsAvailable)
        {
            StartUTC = startUTC;
            EndUTC = endUTC;
            TimeZone = timeZone;
            IsAvailable = isAvailable;
            UnitsAvailable = unitsAvailable;

            Start = timeZone.ConvertTimeFromUtc(StartUTC);
            End = timeZone.ConvertTimeFromUtc(EndUTC);
        }

        public string SuggestionDateMessage(DateTime rentalStartUtc, DateTime rentalEndUtc)
        {
            if (!IsAvailable)
            {
                if (StartUTC >= rentalStartUtc)
                    return GlobalizationHelper.FormatString("RENTAL_TRY_FROM_MESSAGE", FormatHelper.DisplayDateTime(StartUTC, TimeZone));

                return GlobalizationHelper.FormatString("RENTAL_TRY_UNTIL_MESSAGE", FormatHelper.DisplayDateTime(EndUTC, TimeZone));
            }

            return "";
        }

        public string UnitsAvailableMessage()
        {
            if (IsAvailable)
                return GlobalizationHelper.FormatString("RENTAL_UNITS_AVAILABLE_MESSAGE", UnitsAvailable);

            return "";
        }

        public static AvailabilityDates Unavailable(DateTime rentalStartUTC, DateTime rentalEndUTC, MarketplaceTimeZone timeZone,
            DateTime firstLockUTC, DateTime lastLockUTC)
        {
            return new AvailabilityDates(
                startUTC: rentalStartUTC < firstLockUTC ? rentalStartUTC : lastLockUTC,//.AddHours(1),
                endUTC: rentalStartUTC < firstLockUTC ? firstLockUTC.AddHours(-1) : rentalEndUTC,
                timeZone: timeZone,
                isAvailable: false,
                unitsAvailable: 0
            );
        }

        public static AvailabilityDates Unavailable(DateTime rentalStartUTC, DateTime rentalEndUTC, MarketplaceTimeZone timeZone)
        {
            return new AvailabilityDates(
                startUTC: rentalStartUTC,
                endUTC: rentalEndUTC,
                timeZone: timeZone,
                isAvailable: false,
                unitsAvailable: 0
            );
        }

        public static AvailabilityDates Available(DateTime rentalStartUTC, DateTime rentalEndUTC, MarketplaceTimeZone timeZone, int unitsAvailable)
        {
            return new AvailabilityDates(
                rentalStartUTC,
                rentalEndUTC,
                timeZone,
                isAvailable: true,
                unitsAvailable: unitsAvailable
            );
        }
    }
}