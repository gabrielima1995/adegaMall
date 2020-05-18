using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Presentation.Models.ScheduleViewModels
{
    public class NewAvailabilityDayOfWeekVM
    {
        private readonly TimeSpan _minTime = new TimeSpan(0, 0, 0);
        private readonly TimeSpan _maxTime = new TimeSpan(23, 59, 0);
        private readonly TimeSpan _oneMinute = new TimeSpan(0, 1, 0);

        public DayOfWeek DayOfWeek { get; set; }

        public string DayOfWeekLabel => DayOfWeek.ToString().ToUpper() + "_LABEL";

        [DisplayNameLocalized("NEW_AVAILABILITY_START_TIME1_LABEL")]
        [DataType(DataType.Time)]
        [RequiredIfLocalized("EndTime1 != null && EndTime1 > StartTime1")]
        public TimeSpan? StartTime1 { get; set; }

        [DisplayNameLocalized("NEW_AVAILABILITY_END_TIME1_LABEL")]
        [DataType(DataType.Time)]
        [RequiredIfLocalized("StartTime1 != null && EndTime1 > StartTime1")]
        public TimeSpan? EndTime1 { get; set; }

        [DisplayNameLocalized("NEW_AVAILABILITY_START_TIME2_LABEL")]
        [DataType(DataType.Time)]
        [RequiredIfLocalized("EndTime2 != null && StartTime1 != null && EndTime2 > StartTime2")]
        public TimeSpan? StartTime2 { get; set; }

        [DisplayNameLocalized("NEW_AVAILABILITY_END_TIME2_LABEL")]
        [DataType(DataType.Time)]
        [RequiredIfLocalized("StartTime2 != null && StartTime1 != null && EndTime2 > StartTime2")]
        public TimeSpan? EndTime2 { get; set; }

        public NewAvailabilityDayOfWeekVM()
        {
        }

        public NewAvailabilityDayOfWeekVM(DayOfWeek dayOfWeek, List<RentalLock> locks)
        {
            DayOfWeek = dayOfWeek;

            LoadTimes(locks);
        }

        private void LoadTimes(List<RentalLock> locks)
        {
            var starts = new List<TimeSpan>();
            var ends = new List<TimeSpan>();

            if (!locks.Any()) return;

            #region Carrega possíveis horários inicial e final

            foreach (var rentalLock in locks)
            {
                #region Dia todo bloqueado

                if (rentalLock.IsStartOfDay && rentalLock.IsEndOfDay) return;

                #endregion Dia todo bloqueado

                #region Bloqueio no início do dia

                if (rentalLock.IsStartOfDay)
                {
                    var start = rentalLock.EndTime.Value.Add(_oneMinute);

                    starts.Add(start);
                }

                #endregion Bloqueio no início do dia

                #region Bloqueio no fim do dia

                else if (rentalLock.IsEndOfDay)
                {
                    var end = rentalLock.StartTime.Value;

                    ends.Add(end);
                }

                #endregion Bloqueio no fim do dia

                #region Bloqueio de intervalo no dia

                else
                {
                    var start = rentalLock.EndTime.Value.Add(_oneMinute);
                    var end = rentalLock.StartTime.Value;

                    starts.Add(start);
                    ends.Add(end);
                }

                #endregion Bloqueio de intervalo no dia
            }

            #endregion Carrega possíveis horários inicial e final

            // número de bloqueios de intervalo no dia
            var halfCount = locks.Count(i => !i.IsStartOfDay && !i.IsEndOfDay);

            #region Verifica horários inicial e final

            if (!starts.Any() ||
                (starts.Count == halfCount))
            {
                starts.Add(_minTime);
            }

            if (!ends.Any() ||
                (ends.Count == halfCount))
            {
                ends.Add(_maxTime);
            }

            #endregion Verifica horários inicial e final

            #region Somente um horário inicial e final

            if (starts.Count == 1 && ends.Count == 1)
            {
                StartTime1 = starts.First();
                EndTime1 = ends.First();
            }

            #endregion Somente um horário inicial e final

            #region Mais de um horário inicial e final

            else
            {
                starts.Sort();
                ends.Sort();

                StartTime1 = starts.First();
                StartTime2 = starts.Last();

                EndTime1 = ends.First();
                EndTime2 = ends.Last();
            }

            #endregion Mais de um horário inicial e final
        }
    }
}