using System;

namespace NewIntegrator.Stripe.Plans
{
    public class StripePlan
    {
        public string Id { get; internal set; }

        public string Nickname { get; internal set; }

        public long Amount { get; internal set; }

        public string Currency { get; internal set; }

        public StripePlanInterval Interval { get; internal set; }

        public long IntervalCount { get; internal set; }

        public long TrialPeriodDay { get; internal set; }

        public long GetIntervalDays()
        {
            switch (Interval)
            {
                case StripePlanInterval.Day:
                    return IntervalCount;

                case StripePlanInterval.Week:
                    return IntervalCount * 7;

                case StripePlanInterval.Month:
                    return IntervalCount * 30;

                case StripePlanInterval.Year:
                    return IntervalCount * 365;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}