using Domain.Modules.Portfolio;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// Rede social do vendedor
    /// </summary>
    public class SellerSocialMedia
    {
        private DateTime _updatedAt;

        public int Id { get; set; }

        public string ResourceId { get; set; }

        public SocialNetwork SocialNetwork { get; set; }

        public string Nickname { get; set; }

        public string Url { get; set; }

        public int Followers { get; set; }

        public DateTime UpdatedAt
        {
            get { return _updatedAt < DateTime.MinValue ? DateTime.MinValue : _updatedAt; }
            set { _updatedAt = value; }
        }

        [Required]
        public Portfolio Portfolio { get; set; }

        public SellerSocialMedia()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}