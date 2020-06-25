using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.StoreModule.Entities
{
    public enum LockMode
    {
        [DescriptionLocalized("LOCK_TYPE_FULLY_UNAVAILABLE")]
        FullyUnavailable = 0,

        [DescriptionLocalized("LOCK_TYPE_ONLY_FOR_SHIPPING")]
        OnlyForShipping = 1
    }

}