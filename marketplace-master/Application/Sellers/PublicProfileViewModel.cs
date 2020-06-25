using Domain.Entities;

namespace Application.Sellers
{
    public class PublicProfileViewModel
    {
        public Seller Seller { get; set; }

        public bool ShowLinkForMoreProducts { get; set; }

        public string LinkForMoreProducts { get; set; }
    }
}