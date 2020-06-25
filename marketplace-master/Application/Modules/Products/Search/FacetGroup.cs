using System.Collections.Generic;

namespace Application.Modules.Products.Search
{
    public class FacetGroup
    {
        public string Name { get; set; }

        public IList<FacetItem> Items { get; set; }

        public FacetGroup()
        {
            Items = new List<FacetItem>();
        }

        public FacetGroup(string name)
            : this()
        {
            Name = name;
        }
    }
}
