using Dapper;
using Domain.StoreModule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class RentalShiftQueries : IRentalShiftQueries
    {
        public IReadOnlyList<ProductRentalShiftDto> ListRentalShiftsForAllProducts()
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT
	                    RentalShift.Id,
                        ProductRentalShift.ProductId,
	                    RentalShift.Name,
	                    RentalShift.StartHours,
	                    RentalShift.StartMinutes,
	                    RentalShift.EndHours,
	                    RentalShift.EndMinutes
                    FROM RentalShift
	                    INNER JOIN ProductRentalShift ON RentalShift.Id = ProductRentalShift.RentalShiftId
                        INNER JOIN Product ON ProductRentalShift.ProductId = Product.Id
                            AND Product.Deleted = 0 AND Product.CurrentStock > 0
                    WHERE RentalShift.Deleted = 0
                    ORDER BY
                        RentalShift.StartHours, RentalShift.StartMinutes,
                        RentalShift.EndHours, RentalShift.EndMinutes,
	                    RentalShift.Name";

                return conn.Query<ProductRentalShiftDto>(sql).ToList();
            }
        }

        public IReadOnlyList<RentalShiftDto> ListRentalShifts()
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT
	                    RentalShift.Id,
	                    RentalShift.Name,
	                    RentalShift.StartHours,
	                    RentalShift.StartMinutes,
	                    RentalShift.EndHours,
	                    RentalShift.EndMinutes
                    FROM RentalShift
                    WHERE RentalShift.Deleted = 0
                    ORDER BY
                        RentalShift.StartHours, RentalShift.StartMinutes,
                        RentalShift.EndHours, RentalShift.EndMinutes,
	                    RentalShift.Name";

                return conn.Query<RentalShiftDto>(sql).ToList();
            }
        }

        public IReadOnlyList<RentalShiftDto> ListProductShifts(Guid productId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT
	                    RentalShift.Id,
	                    RentalShift.Name,
	                    RentalShift.StartHours,
	                    RentalShift.StartMinutes,
	                    RentalShift.EndHours,
	                    RentalShift.EndMinutes
                    FROM RentalShift
	                    INNER JOIN ProductRentalShift ON RentalShift.Id = ProductRentalShift.RentalShiftId
		                    AND ProductRentalShift.ProductId = @productId
                    WHERE RentalShift.Deleted = 0
                    ORDER BY
                        RentalShift.StartHours, RentalShift.StartMinutes,
                        RentalShift.EndHours, RentalShift.EndMinutes,
	                    RentalShift.Name";

                return conn.Query<RentalShiftDto>(sql, new { productId }).ToList();
            }
        }
    }
}
