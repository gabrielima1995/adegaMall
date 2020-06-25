namespace Application.Modules.Products.Search
{
    public class FacetItem
    {
        public int VariantId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public int Quantity { get; set; }

        public bool Selected { get; set; }
    }
}
