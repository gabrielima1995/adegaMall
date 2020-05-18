using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.DTO
{
    /// <summary>
    /// Representa um dia do mês com os seus horários de locação.
    /// </summary>
    public class RentalDayGridView
    {
        /// <summary>
        /// Dia do mês.
        /// </summary>
        public DateTime Day { get; private set; }

        /// <summary>
        /// Horários de locação.
        /// </summary>
        public List<RentalPeriodGridView> Periods { get; private set; }

        /// <summary>
        /// Número de horários para o dia.
        /// </summary>
        public int TotalPeriods => Periods.Count;

        protected RentalDayGridView()
        {
        }

        /// <summary>
        /// Novo dia do mês com os horários padrão de locação.
        /// </summary>
        /// <param name="day">Dia do mês.</param>
        /// <param name="periodUnit">Unidade do perído de locação.</param>
        /// <param name="currentStock">Total de produtos disponíveis para locação.</param>
        /// <returns></returns>
        protected RentalDayGridView(DateTime day, RentalPeriodUnit periodUnit, int currentStock)
        {
            Day = day;

            var total = periodUnit == RentalPeriodUnit.Hour ? 24 : 24 * 60;

            Periods = new List<RentalPeriodGridView>(total);

            for (var i = 0; i < total; i++)
            {
                var time = GetTime(periodUnit, i);
                Periods.Add(RentalPeriodGridView.New(time, currentStock));
            }
        }

        /// <summary>
        /// Novo dia do mês com os horários padrão de locação.
        /// </summary>
        /// <param name="day">Dia do mês.</param>
        /// <param name="periodUnit">Unidade do perído de locação.</param>
        /// <param name="availabilities">Horários disponíveis para o produto.</param>
        /// <returns></returns>
        protected RentalDayGridView(DateTime day, RentalPeriodUnit periodUnit, IList<ProductAvailability> availabilities)
        {
            Day = day;

            var availabilitiesOfDay = availabilities
                .Where(j => j.InDayOfWeek(day.DayOfWeek))
                .ToArray();

            Periods = new List<RentalPeriodGridView>(availabilitiesOfDay.Length);

            foreach (var availability in availabilitiesOfDay)
            {
                var time = availability.StartTime;

                Periods.Add(RentalPeriodGridView.New(time, availability));
            }
        }

        private TimeSpan GetTime(RentalPeriodUnit periodUnit, int periodValue)
        {
            return periodUnit == RentalPeriodUnit.Hour
                ? TimeSpan.FromHours(periodValue)
                : TimeSpan.FromMinutes(periodValue);
        }

        /// <summary>
        /// Remove horários para determinada condição via função.
        /// </summary>
        /// <param name="func">Função com expressão condicional.</param>
        /// <param name="substractStock">Estoque para remover do horário.</param>
        private void RemoveInternal(Func<RentalPeriodGridView, bool> func, int? substractStock)
        {
            var toRemoveList = new List<RentalPeriodGridView>(Periods.Count);

            Periods.ForEach(i =>
            {
                if (func.Invoke(i))
                {
                    // retira do estoque
                    if (substractStock.HasValue) i.SubstractStock(substractStock.Value);

                    // remove direto
                    if (!substractStock.HasValue) toRemoveList.Add(i);
                }
            });

            toRemoveList.ForEach(i => Periods.Remove(i));
        }

        /// <summary>
        /// Remove horários até certo horário do dia.
        /// </summary>
        /// <param name="time">Horário do dia.</param>
        /// <param name="substractStock">Estoque para remover do horário.</param>
        public void RemovePeriodsUntil(TimeSpan time, int? substractStock = null, Guid? productAvailability = null)
        {
            RemoveInternal(i => i.Time < time && (productAvailability == null || i.Availability == productAvailability), substractStock);
        }

        /// <summary>
        /// Remove horários a partir de um horário do dia.
        /// </summary>
        /// <param name="time">Horário do dia.</param>
        /// <param name="substractStock">Estoque para remover do horário.</param>
        public void RemovePeriodsFrom(TimeSpan time, int? substractStock = null, Guid? productAvailability = null)
        {
            RemoveInternal(i => i.Time >= time && (productAvailability == null || i.Availability == productAvailability), substractStock);
        }

        /// <summary>
        /// Remove horários de um certo horário até outro horário do dia.
        /// </summary>
        /// <param name="timeFrom"></param>
        /// <param name="timeUntil"></param>
        /// <param name="substractStock">Estoque para remover do horário.</param>
        public void RemovePeriodsFromUntil(TimeSpan timeFrom, TimeSpan timeUntil, int? substractStock = null, Guid? productAvailability = null)
        {
            RemoveInternal(i => i.Time >= timeFrom && i.Time < timeUntil && (productAvailability == null || i.Availability == productAvailability), substractStock);
        }

        /// <summary>
        /// Remove todos os horários do dia.
        /// </summary>
        /// <param name="substractStock">Estoque para remover do horário.</param>
        public void RemoveAllPeriods(int? substractStock = null, Guid? productAvailability = null)
        {
            RemoveInternal(i => true && (productAvailability == null || i.Availability == productAvailability), substractStock);
        }

        /// <summary>
        /// Remove todos os horários do dia que não tem estoque disponível para determinada quantidade.
        /// </summary>
        /// <param name="amount">Quantidade para locação.</param>
        public void RemoveAllPeriodsUnavailableStock(int amount)
        {
            Periods.RemoveAll(i => i.IsAvailabilityDeleted || i.CurrentStock < amount);
        }

        /// <summary>
        /// Remove todos os horários do dia que a disponibilidade do produto foi excluída e não tem locações.
        /// </summary>
        public void RemoveAllPeriodsAvailabilitiesDeletedAndRentalsOff()
        {
            Periods.RemoveAll(i => i.IsAvailabilityDeleted && i.CurrentStock == i.OriginalStock);
        }

        /// <summary>
        /// Novo dia do mês com os horários padrão de locação.
        /// </summary>
        /// <param name="day">Dia do mês.</param>
        /// <param name="periodUnit">Unidade do perído de locação.</param>
        /// <param name="currentStock">Total de produtos disponíveis para locação.</param>
        /// <returns></returns>
        public static RentalDayGridView New(DateTime day, RentalPeriodUnit periodUnit, int currentStock)
        {
            return new RentalDayGridView(day, periodUnit, currentStock);
        }

        /// <summary>
        /// Novo dia do mês com os horários padrão de locação.
        /// </summary>
        /// <param name="day">Dia do mês.</param>
        /// <param name="periodUnit">Unidade do perído de locação.</param>
        /// <param name="availabilities">Horários disponíveis para o produto.</param>
        /// <returns></returns>
        public static RentalDayGridView New(DateTime day, RentalPeriodUnit periodUnit, IList<ProductAvailability> availabilities)
        {
            return new RentalDayGridView(day, periodUnit, availabilities);
        }

        public override string ToString()
        {
            return $"{Day:dd/MM/yyyy} - {Periods.Count} horário(s)";
        }
    }
}