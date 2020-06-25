using Dapper;
using Domain.DTO;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.StoreModule.Queries
{
    public class ScheduleQueries : IScheduleQueries, IRentalLockQueries
    {
        public IList<ProductWithScheduleDto> ListProductsWithRentalsAndLocks(string sellerId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var startDate = DateTime.Today.AddDays(-30D);

                var sql = SqlScriptHelper.GetScript("ScheduleQueries_ListProductsWithRentalsAndLocks");
                var products = conn.Query<ProductWithScheduleDto>(sql, new
                {
                    rental = ProductBusinessModel.Rental,
                    cancelled = OrderStatus.Canceled,
                    rejected = OrderStatus.Rejected,
                    sellerId,
                    startDate
                });

                return products.ToList();
            }
        }

        public IList<RentalScheduleItem> ListRentalScheduleItems(Guid? productId, string sellerId, DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone, bool onlyRentals = false, Guid? productAvailabilityId = null)
        {
            // Filtro para bloqueios deve ser no timezone do usuário
            var periodStart = timeZone.ConvertTimeFromUtc(periodStartUtc);
            var periodEnd = timeZone.ConvertTimeFromUtc(periodEndUtc);

            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("ScheduleQueries_ListRentalScheduleItems");
                var query = conn.QueryMultiple(sql, new
                {
                    rental = ProductBusinessModel.Rental,
                    canceled = OrderStatus.Canceled,
                    rejected = OrderStatus.Rejected,
                    defaultType = ProductType.Default,
                    packageType = ProductType.Package,
                    sellerId,
                    startUTC = periodStartUtc,
                    endUTC = periodEndUtc,
                    startDate = periodStart.Date,
                    endDate = periodEnd.Date,
                    productId,
                    productAvailabilityId
                });

                var locks = query.Read<RentalLockForScheduleDto>();
                var rentals = query.Read<RentalForScheduleDto>();

                var results = GetItems(rentals, timeZone);

                if (!onlyRentals)
                {
                    results = results.Concat(GetItems(locks, periodStartUtc, periodEndUtc, timeZone, true));
                }

                return results.ToList();
            }
        }

        public IList<RentalScheduleItem> ListLockRentalItems(Guid productId, string sellerId, DateTime periodStartUTC, DateTime periodEndUTC, MarketplaceTimeZone timeZone, bool onlyFullyUnavailable = true, bool onlyByProduct = false)
        {
            // Filtro para bloqueios deve ser no timezone do usuário
            var periodStart = timeZone.ConvertTimeFromUtc(periodStartUTC);
            var periodEnd = timeZone.ConvertTimeFromUtc(periodEndUTC);

            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("ScheduleQueries_ListLockRentalItems");
                var locks = conn.Query<RentalLockForScheduleDto>(sql, new
                {
                    productId,
                    sellerId,
                    startDate = periodStart,
                    endDate = periodEnd,
                    onlyByProduct
                });

                var results = GetItems(locks, periodStartUTC, periodEndUTC, timeZone, !onlyFullyUnavailable);

                return results
                    .Where(i => i.Intersects(periodStartUTC, periodEndUTC))
                    .ToList();
            }
        }

        public IList<LockProductRentalDto> ListLocksProductRental(Guid productId, string sellerId, DateTime periodStartUTC, DateTime periodEndUTC, MarketplaceTimeZone timeZone, bool onlyFullyUnavailable = true)
        {
            var list = ListLockRentalItems(productId, sellerId, periodStartUTC, periodEndUTC, timeZone, onlyFullyUnavailable);

            return list
                .Select(i => new LockProductRentalDto
                {
                    LockStartUtc = i.StartUTC,
                    LockEndUtc = i.EndUTC
                })
                .ToList();
        }

        private IEnumerable<RentalScheduleItem> GetItems(IEnumerable<RentalLockForScheduleDto> list, DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone, bool isListing)
        {
            // Filtro para bloqueios deve ser no timezone do usuário.
            var periodStart = timeZone.ConvertTimeFromUtc(periodStartUtc);
            var periodEnd = timeZone.ConvertTimeFromUtc(periodEndUtc);

            foreach (var item in list)
            {
                switch (item.Type)
                {
                    // If the lock is for a specific date, just return it.
                    case LockType.ByDate:
                        yield return new RentalScheduleItem(item, timeZone, item.StartDateTimeUTC.Value, item.EndDateTimeUTC.Value);
                        break;

                    // If the lock is recurrent by day of the week, explode it into an array of locks for specific dates.
                    case LockType.ByDayOfWeek:
                        var dateCursor = periodStart.Date;

                        while (dateCursor <= periodEnd)
                        {
                            if (dateCursor.DayOfWeek != item.DayOfWeek)
                            {
                                dateCursor = dateCursor.AddDays(1);
                                continue;
                            }

                            var lockStart = dateCursor.Add(item.StartTime.Value);
                            var lockEnd = dateCursor.Add(item.EndTime.Value);

                            bool shouldReturnLocks = isListing;

                            if (!isListing)
                            {
                                switch (item.Mode)
                                {
                                    case LockMode.FullyUnavailable:
                                        shouldReturnLocks =
                                            (lockStart >= periodStart && lockStart <= periodEnd) ||
                                            (lockEnd >= periodStart && lockEnd <= periodEnd) ||
                                            (lockStart <= periodStart && lockEnd >= periodEnd);
                                        break;

                                    case LockMode.OnlyForShipping:
                                        shouldReturnLocks =
                                            (periodStart >= lockStart && periodStart <= lockEnd) ||
                                            (periodEnd >= lockStart && periodEnd <= lockEnd) ||
                                            (periodStart >= lockStart && periodEnd <= lockEnd);
                                        break;

                                    default:
                                        throw new IndexOutOfRangeException($"Lock Mode {item.Mode} is not handled.");
                                }
                            }

                            if (shouldReturnLocks)
                                yield return new RentalScheduleItem(item, timeZone,
                                    timeZone.ConvertTimeToUtc(lockStart), timeZone.ConvertTimeToUtc(lockEnd));

                            dateCursor = dateCursor.AddDays(7);
                        }

                        break;
                }
            }
        }

        private IEnumerable<RentalScheduleItem> GetItems(IEnumerable<RentalForScheduleDto> list, MarketplaceTimeZone timeZone, bool isScheduleSeller = true)
        {
            return list.Select(item => new RentalScheduleItem(item, timeZone, isScheduleSeller));
        }

        public IList<RentalScheduleItem> ListRentalScheduleBuyerItems(Guid? productId, string buyerId, DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("ScheduleQueries_ListRentalScheduleBuyerItems");
                var query = conn.QueryMultiple(sql, new
                {
                    rental = ProductBusinessModel.Rental,
                    canceled = OrderStatus.Canceled,
                    rejected = OrderStatus.Rejected,
                    defaultType = ProductType.Default,
                    packageType = ProductType.Package,
                    buyerId,
                    startUTC = periodStartUtc,
                    endUTC = periodEndUtc,
                    productId
                });

                var rentals = query.Read<RentalForScheduleDto>();

                var results = GetItems(rentals, timeZone, isScheduleSeller: false);

                return results.ToList();
            }
        }
    }
}