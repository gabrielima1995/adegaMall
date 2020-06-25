using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    public class ApiTokenEditing
    {
        public Guid Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        public string Token { get; set; }

        [Display(Name = "Vendedor")]
        public string SellerId { get; set; }

        public IList<SellerSelectorItem> Sellers { get; set; }
    }
}