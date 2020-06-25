using Domain.Entities;
using Framework;
using System;

namespace Domain.StoreModule.Entities
{
    public class ProductRentalShift : BaseEntity<Guid>
    {
        private ProductRentalShift()
        {
        }

        private ProductRentalShift(Guid productId, Guid rentalShiftId)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            RentalShiftId = rentalShiftId;
        }

        public Guid ProductId { get; private set; }
        public Guid RentalShiftId { get; private set; }

        public static OperationResult<ProductRentalShift> Create(Guid productId, Guid rentalShiftId)
        {
            var errors = new ErrorDictionary();

            if (productId == Guid.Empty)
                errors.Add(nameof(productId), "Product ID must be valid (not empty).");

            if (rentalShiftId == Guid.Empty)
                errors.Add(nameof(productId), "Rental Shift ID must be valid (not empty).");

            if (errors.HasErrors)
                return new OperationResult<ProductRentalShift>(errors);

            return new OperationResult<ProductRentalShift>(new ProductRentalShift(productId, rentalShiftId));
        }
    }
}
