using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.DTO
{
    public enum ProductSortType
    {
        [DescriptionLocalized("SEARCH_SORT_MOST_RELEVANT")]
        MostRelevant = 0,

        [DescriptionLocalized("SEARCH_SORT_LOWER_PRICE")]
        LowerPrice,

        [DescriptionLocalized("SEARCH_SORT_HIGHER_PRICE")]
        HigherPrice,

        [DescriptionLocalized("SEARCH_SORT_ALPHABETICAL")]
        Alphabetical,

        [DescriptionLocalized("SEARCH_SORT_NEWER")]
        Newer,

        [DescriptionLocalized("SEARCH_SORT_LOWER_DISTANCE")]
        Distance
    }
}
