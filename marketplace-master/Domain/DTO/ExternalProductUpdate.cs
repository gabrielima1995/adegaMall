using System;

namespace Domain.DTO
{
    public class ExternalProductUpdate
    {
        public Guid? Id { get; set; }
        public string ExternalId { get; set; }
        public int? Stock { get; set; }
        public decimal? Price { get; set; }
    }
}
