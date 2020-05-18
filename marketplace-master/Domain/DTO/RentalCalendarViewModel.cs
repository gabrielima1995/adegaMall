using Application.Config;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Interfaces;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.DTO
{
    public class RentalCalendarViewModel
    {
        public RentalCalendarViewModel(Product product,
            DateTime startDate, int totalDays, MarketplaceTimeZone timeZone,
            IEnumerable<RentalShiftDto> shifts, IEnumerable<RentalShiftDto> productShifts,
            Func<DateTime, string> dateUrlGenerator)
        {
            this.product = product;

            CurrentUrl = dateUrlGenerator(startDate);
            PreviousUrl = dateUrlGenerator(startDate.AddDays(-7));
            NextUrl = dateUrlGenerator(startDate.AddDays(7));

            var endDate = startDate.AddDays(totalDays);
            var currentStock = (int)product.CurrentStock;

            var lines = new List<RentalCalendarViewModel.CalendarLine>();

            var productShiftsArr = productShifts as RentalShiftDto[] ?? productShifts.ToArray();

            foreach (var shift in shifts)
            {
                var columns = new List<RentalCalendarViewModel.CalendarColumn>();

                for (var date = startDate; date < endDate; date = date.AddDays(1))
                {
                    var startTimeUtc = timeZone.CreateDateTime(date.Year, date.Month, date.Day, shift.StartHours, shift.StartMinutes, 0).UniversalTime;
                    var endTimeUtc = timeZone.CreateDateTime(date.Year, date.Month, date.Day, shift.EndHours, shift.EndMinutes, 0).UniversalTime;

                    var isAvailable = productShiftsArr.Contains(shift);

                    var startTimeLocal = timeZone.ConvertTimeFromUtc(startTimeUtc);
                    var endTimeLocal = timeZone.ConvertTimeFromUtc(endTimeUtc);

                    columns.Add(new RentalCalendarViewModel.CalendarColumn(
                        isAvailable, false,
                        startTimeLocal, endTimeLocal,
                        currentStock));
                }

                lines.Add(new RentalCalendarViewModel.CalendarLine(
                    RentalCalendarViewModel.RentalShiftForCalendarViewModel.From(shift, product.Price), columns));
            }

            Lines = lines;
        }

        private readonly Product product;

        public Guid ProductId => product.Id;
        public bool IsPackageProduct => product.IsPackageType;
        public IReadOnlyList<CalendarLine> Lines { get; }

        public string CurrentUrl { get; }
        public string PreviousUrl { get; }
        public string NextUrl { get; }

        public bool ShowShiftDuration => !MaaSConfig.Current.Product.BusinessModel.Rental.HideShiftDurationFromUsers;

        public IReadOnlyList<DateTime> HeaderDates
        {
            get
            {
                if (Lines == null || Lines.Count == 0)
                    return new List<DateTime>();

                return Lines.First().Columns.Select(column => column.Date).ToList();
            }
        }

        public void UnavailablePeriods(DateTime rangeStart, DateTime rangeEnd, int? substractStock = null)
        {
            foreach (var line in Lines)
            {
                line.UnavailablePeriodsFromUntil(rangeStart, rangeEnd, substractStock);
            }
        }

        public void AdjustAvailableStockPeriod(int amount)
        {
            foreach (var line in Lines)
            {
                line.UnavailableAllPeriodsUnavailableStock(amount);
            }
        }

        public class CalendarLine
        {
            public CalendarLine(RentalShiftForCalendarViewModel shift, IEnumerable<CalendarColumn> columns)
            {
                Shift = shift ?? throw new ArgumentNullException(nameof(shift));

                this.columns = columns?.ToList() ?? throw new ArgumentNullException(nameof(columns));
            }

            public RentalShiftForCalendarViewModel Shift { get; }

            private readonly List<CalendarColumn> columns;
            public IReadOnlyList<CalendarColumn> Columns => columns;

            private void UnavailableInternal(Func<CalendarColumn, bool> func, int? substractStock)
            {
                var toUnavailableList = new List<CalendarColumn>(columns.Count);

                columns.ForEach(i =>
                {
                    if (func.Invoke(i))
                    {
                        // retira do estoque
                        if (substractStock.HasValue) i.SubstractStock(substractStock.Value);

                        // remove direto ou sem estoque
                        if (!substractStock.HasValue || i.CurrentStock <= 0) toUnavailableList.Add(i);
                    }
                });

                toUnavailableList.ForEach(i => i.Unavailable());
            }

            public void UnavailablePeriodsFromUntil(DateTime from, DateTime until, int? substractStock = null)
            {
                UnavailableInternal(i =>
                        from <= i.Start && i.Start < until ||   // start between range
                        from <= i.End && i.End < until ||       // end between range
                        from <= i.Start && i.End < until ||     // start and end between range
                        i.Start <= from && until < i.End,       // range between start and end
                    substractStock);
            }

            public void UnavailableAllPeriods(int? substractStock = null)
            {
                UnavailableInternal(i => true, substractStock);
            }

            public void UnavailableAllPeriodsUnavailableStock(int amount)
            {
                UnavailableInternal(i => i.CurrentStock < amount, null);
            }
        }

        public class CalendarColumn
        {
            public CalendarColumn(bool isAvailable, bool isSelected, DateTime start, DateTime end, int currentStock)
            {
                IsAvailable = isAvailable;
                IsSelected = isSelected;
                Start = start;
                End = end;
                CurrentStock = currentStock;
            }

            public bool IsAvailable { get; private set; }
            public bool IsSelected { get; }
            public DateTime Start { get; }
            public DateTime End { get; }
            public DateTime Date => Start.Date;
            public int CurrentStock { get; private set; }

            public void SubstractStock(int amount)
            {
                CurrentStock -= amount;

                if (CurrentStock <= 0) Unavailable();
            }

            public void Unavailable()
            {
                IsAvailable = false;
            }
        }

        public class RentalShiftForCalendarViewModel
        {
            public string Name { get; set; }
            public string FormattedStartTime { get; set; }
            public string FormattedEndTime { get; set; }
            public string FormattedPrice { get; set; }

            public static RentalShiftForCalendarViewModel From(RentalShiftDto rentalShiftDto, decimal? hourlyRate)
            {
                var start = new DateTime(1, 1, 1, rentalShiftDto.StartHours, rentalShiftDto.StartMinutes, 0);
                var end = new DateTime(1, 1, 1, rentalShiftDto.EndHours, rentalShiftDto.EndMinutes, 0);

                var totalMinutes = (end - start).TotalMinutes;
                var totalPrice = hourlyRate == null ? 0m : (decimal)totalMinutes * hourlyRate.Value / 60m;
                var formattedPrice = totalPrice == 0m ? null : $"{MaaSConfig.Current.DefaultCurrency} {totalPrice.DisplayDecimal(2)}";

                return new RentalShiftForCalendarViewModel
                {
                    Name = rentalShiftDto.Name,
                    FormattedStartTime = start.ToShortTimeString(),
                    FormattedEndTime = end.ToShortTimeString(),
                    FormattedPrice = formattedPrice
                };
            }
        }
    }
}