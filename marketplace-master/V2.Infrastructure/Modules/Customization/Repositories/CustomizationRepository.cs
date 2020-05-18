using Domain.Modules.Customization.Entities;
using Domain.Modules.Customization.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using System.Data.Entity;
using System.Linq;

namespace V2.Infrastructure.Modules.Customization.Repositories
{
    public class CustomizationRepository : EfRepository, ICustomizationRepository
    {
        public CustomizationRepository(ScaffoldContext context) : base(context)
        {
        }

        public PlatformCustomization GetPlatformCustomization()
        {
            return Context.PlatformCustomization.SingleOrDefault();
        }

        public void UpdatePlatformCustomization(PlatformCustomization platformCustomization)
        {
            var custom = Context.PlatformCustomization.SingleOrDefault();

            if (custom == null)
            {
                Context.PlatformCustomization.Add(platformCustomization);
            }
            else
            {
                Context.Entry(platformCustomization).State = EntityState.Modified;
            }
        }
    }
}
