using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    public class ApiTokenListing
    {
        public Guid Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        public string Token { get; set; }

        [Display(Name = "Vendedor")]
        public string SellerName { get; set; }
    }
}