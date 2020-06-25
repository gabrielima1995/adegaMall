using System;

namespace Domain.Entities
{
    /// <summary>
    /// Item personalizado para exibição na home
    /// </summary>
    public class CustomItem : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string TargetUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid CategoryId { get; set; }

        public CustomItem() : base()
        {
            Id = Guid.NewGuid();
        }
    }
}