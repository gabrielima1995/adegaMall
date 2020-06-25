using Domain.Modules.StoreModule.Commands;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Interfaces;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Linq;

namespace Domain.Modules.StoreModule
{
    public class ProductAvailabilityService
    {
        private readonly IProductAvailabilityRepository _productAvailabilityRepository;

        private static readonly TimeSpan OneMinute = new TimeSpan(0, 0, 1, 0);

        public ProductAvailabilityService(
            IProductAvailabilityRepository productAvailabilityRepository)
        {
            _productAvailabilityRepository = productAvailabilityRepository;
        }

        public ProductAvailability GetById(Guid id, string sellerId)
        {
            return _productAvailabilityRepository.Get(
                i => i.Id == id && i.Product.Store_SellerId == sellerId,
                i => i.Product);
        }

        public OperationResult CheckConflict(NewProductAvailabilityCommand command)
        {
            var items = _productAvailabilityRepository.GetListByProduct(command.ProductId, command.DayOfWeek, false);

            var start = command.StartTime;
            var end = command.EndTime.Subtract(OneMinute);

            if (items.Any(i =>
                (start == i.StartTime || i.StartTime == end) ||
                (start == i.EndTime.Subtract(OneMinute) || i.EndTime.Subtract(OneMinute) == end)))
                return OperationResult.Fail(GlobalizationHelper.GetString("PRODUCT_AVAILABILITY_CONFLICT_ERROR"));

            return OperationResult.Success;
        }

        public OperationResult NewAvailability(NewProductAvailabilityCommand command, bool saveChanges = true)
        {
            var entity = ProductAvailability.New(command);

            return _productAvailabilityRepository.Add(entity, saveChanges);
        }

        public OperationResult UpdateAvailability(EditProductAvailabilityCommand command, bool saveChanges = true)
        {
            var availability = _productAvailabilityRepository.GetById(command.AvailabilityId);

            if (availability == null) return OperationResult.Fail("Availability not found");

            availability.Update(command);

            return _productAvailabilityRepository.Update(availability, saveChanges);
        }

        public OperationResult DeleteAvailability(DeleteProductAvailabilityCommand command, bool saveChanges = true)
        {
            var availability = _productAvailabilityRepository.GetById(command.AvailabilityId);

            if (availability == null) return OperationResult.Fail("Availability not found");

            return DeleteAvailability(availability, saveChanges);
        }

        public OperationResult DeleteAvailability(ProductAvailability availability, bool saveChanges = true)
        {
            availability.Delete();

            return _productAvailabilityRepository.Update(availability, saveChanges);
        }

        #region Recurring

        public OperationResult NewAvailability(NewRecurringProductAvailabilityCommand command, bool saveChanges = true)
        {
            var entity = ProductAvailability.New(command);

            return _productAvailabilityRepository.Add(entity, saveChanges);
        }

        public OperationResult UpdateAvailability(EditRecurringProductAvailabilityCommand command, bool saveChanges = true)
        {
            var availability = _productAvailabilityRepository.GetById(command.AvailabilityId);

            if (availability == null) return OperationResult.Fail("Availability not found");

            availability.Update(command);

            return _productAvailabilityRepository.Update(availability, saveChanges);
        }

        #endregion Recurring

        public OperationResult Commit()
        {
            return _productAvailabilityRepository.SaveChanges();
        }
    }
}