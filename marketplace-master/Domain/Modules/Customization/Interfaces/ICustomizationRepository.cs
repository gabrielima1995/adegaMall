using Domain.Modules.Customization.Entities;

namespace Domain.Modules.Customization.Interfaces
{
    public interface ICustomizationRepository
    {
        PlatformCustomization GetPlatformCustomization();

        void UpdatePlatformCustomization(PlatformCustomization platformCustomization);
    }
}
