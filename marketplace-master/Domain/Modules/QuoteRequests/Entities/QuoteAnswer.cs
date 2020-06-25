using Domain.QuoteRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.PortfolioModule
{
    public class QuoteAnswer : BaseEntity<Guid>
    {
        public string Text { get; set; }

        public decimal? Price { get; set; }

        [Required]
        [MaxLength(128)]
        public string AuthorId { get; set; }

        [Required]
        public Guid Quote_Id { get; set; }

        [Required]
        public string Buyer_Id { get; set; }

        [Required]
        public string Seller_Id { get; set; }

        [ForeignKey("Quote_Id")]
        public Quote Quote { get; set; }

        [ForeignKey("Buyer_Id")]
        public Buyer Buyer { get; set; }

        [ForeignKey("Seller_Id")]
        public Seller Seller { get; set; }

        public DateTime? DeliveryDeadline { get; set; }

        public List<QuoteAttachment> Attachments { get; set; } = new List<QuoteAttachment>();

        public QuoteAnswer() { }

        public QuoteAnswer(Guid id, Guid quoteId, string sellerId, string buyerId, string authorId, string text, decimal? price, DateTime? deadline) : this()
        {
            Id = id;
            Quote_Id = quoteId;
            Seller_Id = sellerId;
            Buyer_Id = buyerId;
            AuthorId = authorId;
            Text = text;
            Price = price;
            DeliveryDeadline = deadline;
        }
    }
}
