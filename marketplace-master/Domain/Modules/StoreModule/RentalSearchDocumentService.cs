using Application.Config;
using Domain.Interfaces.Repositories;
using Domain.Services.StoreModule;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Domain.Modules.StoreModule
{
    public class RentalSearchDocumentService
    {
        private const int daysToGrid = 365;

        private readonly IProductRepository _productRepository;
        private readonly RentalService _rentalService;

        public RentalSearchDocumentService(
            IProductRepository productRepository,
            RentalService rentalService)
        {
            _productRepository = productRepository;
            _rentalService = rentalService;
        }

        public RentalSearchDocument GetDocument(Guid productId)
        {
            if (!MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
                return RentalSearchDocument.Empty();

            var product = _productRepository.Get(i => i.Id == productId, i => i.Store.Seller);

            var getRentalGridOperation = _rentalService.GetRentalGrid(
                product, 1, DateTime.UtcNow,
                product.Store.Seller.PreferredTimeZone, totalDaysToGrid: daysToGrid);

            if (getRentalGridOperation.IsFailure)
                return RentalSearchDocument.Empty();

            var grid = getRentalGridOperation.Result;
            var entries = new List<RentalSearchDocumentEntry>(grid.Days.Count);

            foreach (var day in grid.Days)
            {
                var morning = day.Periods.Any(period => RentalSearchDocumentPeriods.Morning.Contains(period.Time));
                var afternoon = day.Periods.Any(period => RentalSearchDocumentPeriods.Afternoon.Contains(period.Time));
                var night = day.Periods.Any(period => RentalSearchDocumentPeriods.Night.Contains(period.Time));

                if (morning || afternoon || night)
                    entries.Add(RentalSearchDocumentEntry.Create(day.Day, morning, afternoon, night));
            }

            return new RentalSearchDocument(entries);
        }
    }

    [DebuggerDisplay("{Entries.Count} entries")]
    public class RentalSearchDocument
    {
        public RentalSearchDocument(IEnumerable<RentalSearchDocumentEntry> entries)
        {
            Entries = (entries ?? Array.Empty<RentalSearchDocumentEntry>()).ToList().AsReadOnly();
        }

        public static RentalSearchDocument Empty() => new RentalSearchDocument(null);
        public ReadOnlyCollection<RentalSearchDocumentEntry> Entries { get; }
    }

    [DebuggerDisplay("{Date}, Wknd {IsWeekend}, MonToFri {IsMondayToFriday}, Mor {AvailableMorning}, Aft {AvailableAfternoon}, Nig {AvailableNight}")]
    public class RentalSearchDocumentEntry
    {
        private RentalSearchDocumentEntry(DateTime date,
            bool isWeekend, bool isMondayToFriday,
            bool availableMorning, bool availableAfternoon, bool availableNight)
        {
            Date = date;
            IsWeekend = isWeekend;
            IsMondayToFriday = isMondayToFriday;
            AvailableMorning = availableMorning;
            AvailableAfternoon = availableAfternoon;
            AvailableNight = availableNight;
        }

        public static RentalSearchDocumentEntry Create(DateTime date, bool availableMorning, bool availableAfternoon, bool availableNight)
        {
            date = new DateTime(date.Year, date.Month, date.Day);
            var isWeekend = date.DayOfWeek.In(DayOfWeek.Saturday, DayOfWeek.Sunday);
            return new RentalSearchDocumentEntry(date, isWeekend, !isWeekend, availableMorning, availableAfternoon, availableNight);
        }

        public DateTime Date { get; }
        public bool IsWeekend { get; }
        public bool IsMondayToFriday { get; }
        public bool AvailableMorning { get; }
        public bool AvailableAfternoon { get; }
        public bool AvailableNight { get; }
    }

    public class RentalSearchDocument_Old
    {
        public List<DateTime> AllPeriods { get; } = new List<DateTime>();
        public List<DateTime> AvailableOnWeekends { get; } = new List<DateTime>();
        public List<DateTime> AvailableMondayToFriday { get; } = new List<DateTime>();
        public List<DateTime> AvailableMorning { get; } = new List<DateTime>();
        public List<DateTime> AvailableAfternoon { get; } = new List<DateTime>();
        public List<DateTime> AvailableNight { get; } = new List<DateTime>();

        public static RentalSearchDocument_Old Empty() => new RentalSearchDocument_Old();

        public void Add(DateTime dateTime)
        {
            var date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            AddIfNew(date, AllPeriods);

            if (dateTime.DayOfWeek.In(DayOfWeek.Saturday, DayOfWeek.Sunday))
                AddIfNew(date, AvailableOnWeekends);

            else AddIfNew(date, AvailableMondayToFriday);

            if (RentalSearchDocumentPeriods.Morning.Contains(dateTime)) AddIfNew(date, AvailableMorning);
            if (RentalSearchDocumentPeriods.Afternoon.Contains(dateTime)) AddIfNew(date, AvailableAfternoon);
            if (RentalSearchDocumentPeriods.Night.Contains(dateTime)) AddIfNew(date, AvailableNight);
        }

        private void AddIfNew(DateTime dateTime, List<DateTime> target)
        {
            if (!target.Contains(dateTime))
                target.Add(dateTime);
        }
    }

    [DebuggerDisplay("{start} to {end}")]
    public class RentalSearchDocumentPeriod
    {
        private readonly TimeSpan start;
        private readonly TimeSpan end;

        internal RentalSearchDocumentPeriod(TimeSpan start, TimeSpan end)
        {
            this.start = start;
            this.end = end;
        }

        public bool Contains(TimeSpan time) => start <= time && time <= end;
        public bool Contains(DateTime dateTime) => Contains(new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second));
    }

    public static class RentalSearchDocumentPeriods
    {
        private static readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1);
        public static RentalSearchDocumentPeriod Morning { get; } = new RentalSearchDocumentPeriod(TimeSpan.FromHours(6), TimeSpan.FromHours(12).Subtract(oneSecond));
        public static RentalSearchDocumentPeriod Afternoon { get; } = new RentalSearchDocumentPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(18).Subtract(oneSecond));
        public static RentalSearchDocumentPeriod Night { get; } = new RentalSearchDocumentPeriod(TimeSpan.FromHours(18), TimeSpan.FromHours(24).Subtract(oneSecond));
    }
}