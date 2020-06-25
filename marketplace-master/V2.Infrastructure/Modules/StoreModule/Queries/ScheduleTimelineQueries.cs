using Dapper;
using Domain.Interfaces.Repositories;
using Domain.Services.StoreModule;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.StoreModule.Queries
{
    public class ScheduleTimelineQueries : IScheduleTimelineQueries
    {
        private readonly IProductRepository productRepository;
        private readonly IScheduleQueries scheduleQueries;
        private readonly RentalService rentalService;

        public ScheduleTimelineQueries(
            IProductRepository productRepository,
            IScheduleQueries scheduleQueries,
            RentalService rentalService)
        {
            this.productRepository = productRepository;
            this.scheduleQueries = scheduleQueries;
            this.rentalService = rentalService;
        }

        private IEnumerable<RentalScheduleTimelineResourceItem> GetResources(IEnumerable<RentalScheduleTimelineProductDto> list)
        {
            return list.Select(RentalScheduleTimelineResourceItem.New);
        }

        public List<RentalScheduleTimelineResourceItem> ListRentalScheduleTimelineResources(Guid? productId, string sellerId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("ScheduleQueries_ListRentalScheduleTimelineResources");
                var query = conn.QueryMultiple(sql, new
                {
                    sellerId,
                    productId
                });

                var products = query.Read<RentalScheduleTimelineProductDto>();

                var results = GetResources(products);

                return results.ToList();
            }
        }

        public List<RentalScheduleItem> ListRentalScheduleTimelineItems(Guid? productId, string sellerId, 
            DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone, 
            bool showLocks = true, bool forceMinStartDate = false, bool shortData = false)
        {
            var periodStart = timeZone.ConvertTimeFromUtc(periodStartUtc);
            var totalDaysToGrid = (int)Math.Floor((periodEndUtc - periodStartUtc).TotalDays);

            var products = productRepository.ListByScheduleTimeline(productId, sellerId);

            var list = new List<RentalScheduleItem>();

            var onlyByProduct = false;

            foreach (var product in products)
            {
                if (showLocks)
                {
                    #region Locks

                    var locks = scheduleQueries.ListLockRentalItems(product.Id, sellerId,
                        periodStartUtc, periodEndUtc, timeZone, false, onlyByProduct);

                    list.AddRange(locks);

                    onlyByProduct = true;

                    #endregion Locks
                }

                #region Rental grid

                var getRentalGridOperation = rentalService.GetRentalGrid(
                    product, 0,
                    periodStart, timeZone,
                    totalDaysToGrid: totalDaysToGrid, onlyAvailableStock: false, forceMinStartDate: forceMinStartDate);

                if (getRentalGridOperation.IsFailure) continue;

                var grid = getRentalGridOperation.Result;

                foreach (var day in grid.Days)
                {
                    foreach (var period in day.Periods)
                    {
                        list.Add(new RentalScheduleItem(product, day, period, timeZone, shortData));
                    }
                }

                #endregion Rental grid
            }

            return list;
        }
    }
}