using Domain.Modules.StoreModule;
using Framework;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class DeleteProductAvailabilityAppCommandHandler
    {
        private readonly ProductAvailabilityService _productAvailabilityService;

        public DeleteProductAvailabilityAppCommandHandler(
            ProductAvailabilityService productAvailabilityService)
        {
            _productAvailabilityService = productAvailabilityService;
        }

        public OperationResult Handle(DeleteProductAvailabilityAppCommand appCommand)
        {
            var availability = _productAvailabilityService.GetById(appCommand.AvailabilityId, appCommand.SellerId);

            if (availability == null) return OperationResult.Fail("Availability not found");

            var operation = _productAvailabilityService.DeleteAvailability(availability, false);
            if (operation.IsFailure) return operation;

            _productAvailabilityService.Commit();

            return OperationResult.Success;
        }
    }
}