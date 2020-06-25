using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Modules.Ratings
{
    [NotMapped]
    public class UserRatingModel : UserRating
    {
        [NotMapped]
        public string SenderName { get; set; }

        [NotMapped]
        public string SenderRole { get; set; }

        [NotMapped]
        public string TargetName { get; set; }

        [NotMapped]
        public string TargetRole { get; set; }

        public string LocalizedRole(string role)
        {
            return role == "Buyer" ? "Comprador" : "Vendedor";
        }
    }
}