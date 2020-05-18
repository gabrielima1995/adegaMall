using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.DTO
{
    /// <summary>
    /// Representa uma grade de horários de locação.
    /// </summary>
    public class RentalGridView
    {
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
        private static readonly TimeSpan FinalHour = TimeSpan.FromMinutes(59);

        /// <summary>
        /// Dados do produto.
        /// </summary>
        public Product Product { get; private set; }

        /// <summary>
        /// Dias do mês com seus horários de locação.
        /// </summary>
        public IList<RentalDayGridView> Days { get; private set; }

        protected RentalGridView()
        {
        }

        /// <summary>
        /// Nova grade de horários.
        /// </summary>
        /// <param name="product">Dados do produto.</param>
        /// <param name="dateStart">Data de início da grade.</param>
        /// <param name="totalDays">Total de dias da grade.</param>
        /// <param name="currentStock">Total de produtos disponíveis para locação.</param>
        protected RentalGridView(Product product, DateTime dateStart, int totalDays, int currentStock)
        {
            Product = product;
            Days = new List<RentalDayGridView>(totalDays);

            var periodUnit = GetPeriodUnit();

            for (var i = 0; i < totalDays; i++)
            {
                Days.Add(RentalDayGridView.New(dateStart.Date.AddDays(i), periodUnit, currentStock));
            }
        }

        /// <summary>
        /// Nova grade de horários.
        /// </summary>
        /// <param name="product">Dados do produto.</param>
        /// <param name="dateStart">Data de início da grade.</param>
        /// <param name="totalDays">Total de dias da grade.</param>
        /// <param name="availabilities">Horários disponíveis para o produto.</param>
        protected RentalGridView(Product product, DateTime dateStart, int totalDays, IList<ProductAvailability> availabilities)
        {
            Product = product;
            Days = new List<RentalDayGridView>(totalDays);

            var periodUnit = GetPeriodUnit();

            for (var i = 0; i < totalDays; i++)
            {
                Days.Add(RentalDayGridView.New(dateStart.Date.AddDays(i), periodUnit, availabilities));
            }
        }

        /// <summary>
        /// Nova grade de horários.
        /// </summary>
        /// <param name="product">Dados do produto.</param>
        /// <param name="dateStart">Data de início da grade.</param>
        /// <param name="totalDays">Total de dias da grade.</param>
        /// <param name="currentStock">Total de produtos disponíveis para locação.</param>
        public static RentalGridView New(Product product, DateTime dateStart, int totalDays, int currentStock)
        {
            return new RentalGridView(product, dateStart, totalDays, currentStock);
        }

        /// <summary>
        /// Nova grade de horários.
        /// </summary>
        /// <param name="product">Dados do produto.</param>
        /// <param name="dateStart">Data de início da grade.</param>
        /// <param name="totalDays">Total de dias da grade.</param>
        /// <param name="availabilities">Horários disponíveis para o produto.</param>
        public static RentalGridView New(Product product, DateTime dateStart, int totalDays, IList<ProductAvailability> availabilities)
        {
            return new RentalGridView(product, dateStart, totalDays, availabilities);
        }

        private RentalPeriodUnit GetPeriodUnit()
        {
            return Product.RentalSettings.RentalPeriodUnit ??
                   MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodUnit;
        }

        /// <summary>
        /// Remove horários da grade num determinado intervalo.
        /// </summary>
        /// <param name="rangeStart">Data e hora inicial.</param>
        /// <param name="rangeEnd">Data e hora final.</param>
        /// <param name="substractStock">Estoque para remover do horário.</param>
        public void RemovePeriods(DateTime rangeStart, DateTime rangeEnd, int? substractStock = null, Guid? productAvailability = null)
        {
            foreach (var gridDay in Days)
            {
                // Início da remoção é no mesmo dia da grade
                if (rangeStart.Date == gridDay.Day)
                {
                    // Fim da remoção é no mesmo dia da grade
                    if (rangeEnd.Date == gridDay.Day)
                    {
                        // Remove os horários entre o início e o fim da remoção
                        gridDay.RemovePeriodsFromUntil(rangeStart.TimeOfDay, rangeEnd.TimeOfDay, substractStock, productAvailability);
                    }
                    // Fim da remoção é posterior ao dia da grade
                    else if (rangeEnd.Date > gridDay.Day)
                    {
                        // Remove os horários a partir horário de início da remoção
                        gridDay.RemovePeriodsFrom(rangeStart.TimeOfDay, substractStock, productAvailability);
                    }
                }
                // Início da remoção é anterior ao dia da grade
                else if (rangeStart.Date < gridDay.Day)
                {
                    // Fim da remoção é no mesmo dia da grade
                    if (rangeEnd.Date == gridDay.Day)
                    {
                        // Remove os horários até o horário de fim da remoção
                        gridDay.RemovePeriodsUntil(rangeEnd.TimeOfDay, substractStock, productAvailability);
                    }
                    // Fim da remoção é posterior ao dia da grade
                    else if (rangeEnd.Date > gridDay.Day)
                    {
                        // Remove todos os horários do dia da grade
                        gridDay.RemoveAllPeriods(substractStock, productAvailability);
                    }
                }
            }
        }

        /// <summary>
        /// Remove os horários que estão dentro da duração fixa.
        /// </summary>
        /// <param name="fixedPeriod">Duração fixa da locação.</param>
        /// <param name="unit">Unidade do período de locação.</param>
        public void AdjustFixedPeriod(TimeSpan fixedPeriod, RentalPeriodUnit unit)
        {
            if (fixedPeriod == TimeSpan.Zero ||
                unit == RentalPeriodUnit.Hour && fixedPeriod == OneHour ||
                unit == RentalPeriodUnit.Minute && fixedPeriod == OneMinute) return;

            foreach (var gridDay in Days)
            {
                var firstPeriodOfTheDay = gridDay.Periods.FirstOrDefault();
                if (firstPeriodOfTheDay == null) continue;

                var lastPeriodOfTheDay = gridDay.Periods.Last();
                var lastPossibleTime = lastPeriodOfTheDay.Time.Add(unit == RentalPeriodUnit.Hour ? FinalHour : TimeSpan.Zero);

                var nextFixedPeriodStartTime = TimeSpan.Zero;

                var maxIterations = 288; // 5 min (24 * 60 / 5)
                var iterationCount = 0;
                while (iterationCount++ < maxIterations)
                {
                    var nextPeriod = gridDay.Periods.FirstOrDefault(i => i.Time >= nextFixedPeriodStartTime);

                    if (nextPeriod == null) break;

                    var fixedPeriodStartTime = nextPeriod.Time;
                    var fixedPeriodEndTime = fixedPeriodStartTime + fixedPeriod - OneMinute;

                    // horário inicial da próxima duração fixa
                    nextFixedPeriodStartTime = fixedPeriodStartTime + fixedPeriod;

                    // duração fixa disponível no dia
                    if (fixedPeriodEndTime <= lastPossibleTime)
                    {
                        // remove entre horários dessa duração fixa
                        gridDay.RemovePeriodsFromUntil(fixedPeriodStartTime + OneMinute, nextFixedPeriodStartTime);
                    }
                    else
                    {
                        // remove todos os horários dessa duração fixa, inclusive
                        gridDay.RemovePeriodsFrom(fixedPeriodStartTime);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove os horários que não tem estoque disponível para determinada quantidade.
        /// </summary>
        /// <param name="amount">Quantidade para locação.</param>
        public void AdjustAvailableStockPeriod(int amount)
        {
            foreach (var gridDay in Days)
            {
                gridDay.RemoveAllPeriodsUnavailableStock(amount);
            }
        }

        /// <summary>
        /// Remove os horários que a disponibilidade do produto já está excluída, sem locações.
        /// </summary>
        public void AdjustAvailabilitiesDeletedAndRentalsOffPeriods()
        {
            foreach (var gridDay in Days)
            {
                gridDay.RemoveAllPeriodsAvailabilitiesDeletedAndRentalsOff();
            }
        }

        public override string ToString()
        {
            return $"{Days.First().Day:dd/MM/yyyy} a {Days.Last().Day:dd/MM/yyyy}";
        }
    }
}