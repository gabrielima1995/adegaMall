using Domain.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class ApiTokenCreateVM
    {
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Vendedor")]
        public string SellerId { get; set; }

        public IList<SellerSelectorItem> Sellers { get; set; }
    }
}