using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Models.ProductViewModels
{
    public class MonthOptionsBuilder
    {
        private readonly DateTime selectedDate;
        private readonly Func<DateTime, string> monthUrlGenerator;

        public MonthOptionsBuilder(DateTime selectedDate, Func<DateTime, string> monthUrlGenerator)
        {
            this.selectedDate = selectedDate;
            this.monthUrlGenerator = monthUrlGenerator;
        }

        public SelectList Build()
        {
            var options = GenerateMonthOptions();
            var selectedOption = options.FirstOrDefault(opt => opt.IsDateInMonth(selectedDate)) ?? options.First();
            return new SelectList(options, nameof(MonthOption.OptionValue), nameof(MonthOption.OptionText), selectedOption.OptionValue);
        }

        private List<MonthOption> GenerateMonthOptions()
        {
            var current = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            return Enumerable.Range(0, 11)
                .Select(monthAdd => current.AddMonths(monthAdd))
                .Select(monthDate => new MonthOption(monthDate.Year, monthDate.Month, monthUrlGenerator))
                .ToList();
        }

        private class MonthOption
        {
            private readonly DateTime monthStart;
            private readonly DateTime firstValidWeek;
            private readonly Func<DateTime, string> monthUrlGenerator;

            public MonthOption(int year, int month, Func<DateTime, string> monthUrlGenerator)
            {
                monthStart = new DateTime(year, month, 1);
                firstValidWeek = GetFirstWeekWithValidDays(monthStart);
                this.monthUrlGenerator = monthUrlGenerator;
            }

            public string OptionText => monthStart.ToString("MMMM yyyy");
            public string OptionValue => monthUrlGenerator(firstValidWeek);

            public bool IsDateInMonth(DateTime date)
            {
                return date == firstValidWeek ||
                    (date.Year == monthStart.Year && date.Month == monthStart.Month);
            }

            private DateTime GetFirstWeekWithValidDays(DateTime date)
            {
                date = date.AddDays((double)date.DayOfWeek * -1); // start of the week

                var daysInAWeek = 7;
                var maxWeeksInMonth = 5;

                var today = DateTime.Today;
                var weeks = Enumerable.Range(0, maxWeeksInMonth).Select(weeksToAdd => date.AddDays(weeksToAdd * daysInAWeek));

                foreach (var week in weeks)
                {
                    var weekDates = Enumerable.Range(0, daysInAWeek).Select(daysToAdd => week.AddDays(daysToAdd));

                    foreach (var weekDate in weekDates)
                        if (weekDate >= today)
                            return week;
                }

                throw new Exception("RentalCalendarViewModel.MonthOptionsBuilder: No valid (selectable) days found in this month.");
            }
        }
    }
}