using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.StoreModule.Entities
{
    public class OrderProductRentalsInfo : BaseEntity<Guid>, IOrderProductRentalInfo
    {
        /// <summary>
        /// Produto do pedido.
        /// </summary>
        public OrderProduct OrderProduct { get; set; }

        [ForeignKey("OrderProduct")]
        public Guid? OrderProduct_Id { get; set; }

        public DateTime? StartDate { get; private set; }

        public DateTime? EndDate { get; private set; }

        public RentalPeriodUnit? RentalPeriodUnit { get; private set; }

        public Guid? ProductAvailability_Id { get; private set; }

        [ForeignKey("ProductAvailability_Id")]
        public ProductAvailability ProductAvailability { get; private set; }

        public decimal? RentalPeriodValue { get; private set; }

        public bool? ChargeDoubleShipping { get; private set; }

        public bool? IsReturned { get; private set; }

        public DateTime? ReturnedAt { get; private set; }

        public bool HasValue => StartDate != null && EndDate != null && RentalPeriodUnit != null;

        public OrderProductRentalsInfo()
        {
        }

        public OrderProductRentalsInfo(OrderProduct orderProduct, DateTime startDate, DateTime endDate, Guid? productAvailabilityId)
        {
            Id = Guid.NewGuid();
            OrderProduct = orderProduct;
            OrderProduct_Id = orderProduct.Id;
            StartDate = startDate;
            EndDate = endDate;
            ProductAvailability_Id = productAvailabilityId;
            RentalPeriodUnit = orderProduct.RentalInfo.RentalPeriodUnit;
            RentalPeriodValue = orderProduct.RentalInfo.RentalPeriodValue;
            ChargeDoubleShipping = orderProduct.RentalInfo.ChargeDoubleShipping;
        }

        public void ChangePeriod(DateTime rentalStartUtc, DateTime rentalEndUtc, Guid? productAvailabilityId)
        {
            if (rentalStartUtc >= rentalEndUtc)
                throw new ArgumentException("End date must come after the start date.");

            StartDate = rentalStartUtc;
            EndDate = rentalEndUtc;
            ProductAvailability_Id = productAvailabilityId;
        }

        public void SetAsReturned(DateTime? returnedAt = null)
        {
            IsReturned = true;
            ReturnedAt = returnedAt ?? DateTime.UtcNow;
        }
    }
}