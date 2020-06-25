using Domain.Core.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Modules.Customization.Entities;
using Domain.Modules.Customization.Interfaces;

namespace Domain.Modules.Customization
{
    public class CustomizationService
    {
        private readonly ICustomizationRepository _customizationRepository;
        private readonly IUnitOfWork _uow;

        public CustomizationService(ICustomizationRepository customizationRepository, IUnitOfWork uow)
        {
            _customizationRepository = customizationRepository;
            _uow = uow;
        }

        public PlatformCustomization GetPlatformCustomization()
        {
            return _customizationRepository.GetPlatformCustomization() ?? PlatformCustomization.Create();
        }

        public void UpdatePlatformCustomization(PlatformCustomization platformCustomization)
        {
            _customizationRepository.UpdatePlatformCustomization(platformCustomization);

            var result = _uow.Save();
            if (!result.OK) throw new OperationResultException(result);
        }
    }
}
