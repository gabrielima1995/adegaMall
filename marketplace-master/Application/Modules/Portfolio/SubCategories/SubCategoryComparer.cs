using System.Collections.Generic;

namespace Application.Modules.Portfolio.SubCategories
{
    public class SubCategoryComparer : IEqualityComparer<SubCategoryAndCategorySelector>
    {
        public bool Equals(SubCategoryAndCategorySelector x, SubCategoryAndCategorySelector y)
        {
            return x.Category == y.Category && x.SubCategory == y.SubCategory;
        }

        public int GetHashCode(SubCategoryAndCategorySelector obj)
        {
            return obj.SubCategory ?? 0 + obj.Category ?? 0;
        }
    }
}