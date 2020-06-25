using Domain.Modules.StoreModule;
using Domain.Modules.StoreModule.Commands;
using Domain.Services;
using Framework;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class EditProductAvailabilityAppCommandHandler
    {
        private readonly ProductAvailabilityService _productAvailabilityService;
        private readonly SellerService _sellerService;

        public EditProductAvailabilityAppCommandHandler(
            ProductAvailabilityService productAvailabilityService,
            SellerService sellerService)
        {
            _productAvailabilityService = productAvailabilityService;
            _sellerService = sellerService;
        }

        public OperationResult Handle(EditProductAvailabilityAppCommand appCommand)
        {
            var seller = _sellerService.GetById(appCommand.SellerId);
            var availability = _productAvailabilityService.GetById(appCommand.AvailabilityId, appCommand.SellerId);

            if (availability == null) return OperationResult.Fail("Availability not found");

            var command = new EditProductAvailabilityCommand(
                availability, seller,
                appCommand.CurrentStock, appCommand.DiscountPercentage);

            var operation = _productAvailabilityService.UpdateAvailability(command, false);
            if (operation.IsFailure) return operation;

            _productAvailabilityService.Commit();

            return OperationResult.Success;
        }

        public OperationResult Handle(EditRecurringProductAvailabilityAppCommand appCommand)
        {
            var seller = _sellerService.GetById(appCommand.SellerId);
            var availability = _productAvailabilityService.GetById(appCommand.AvailabilityId, appCommand.SellerId);

            if (availability == null) return OperationResult.Fail("Availability not found");

            var command = new EditRecurringProductAvailabilityCommand(
                availability, seller,
                appCommand.CurrentStock, appCommand.DiscountPercentage);

            var operation = _productAvailabilityService.UpdateAvailability(command, false);
            if (operation.IsFailure) return operation;

            _productAvailabilityService.Commit();

            return OperationResult.Success;
        }
    }
}