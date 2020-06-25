using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    /// <summary>
    /// Avaliação pública de um usuário
    /// </summary>
    public class UserRating : BaseEntity<Guid>
    {
        public string RatingTargetId { get; set; }

        public string RatingSenderId { get; set; }

        public bool Hidden { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// Avaliação (1 a 5 estrelas)
        /// </summary>
        public int? Rating { get; set; }

        public Order Order { get; set; }

        public Quote Quote { get; set; }

        [ForeignKey("Order")]
        public Guid? OrderId { get; set; }

        [ForeignKey("Quote")]
        public Guid? QuoteId { get; set; }

        //Double-blind model
        public Guid? PairedRating { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsPairCompleted { get; set; }

        public bool IsVisible
        {
            get
            {
                if (Hidden ||
                    !Rating.HasValue ||
                    string.IsNullOrWhiteSpace(Comment) ||
                    PairedRating.HasValue && !IsPairCompleted ||
                    PairedRating.HasValue && !IsCompleted)
                    return false;

                return true;
            }
        }

        public UserRating() : base()
        {
            Id = Guid.NewGuid();
        }
    }
}