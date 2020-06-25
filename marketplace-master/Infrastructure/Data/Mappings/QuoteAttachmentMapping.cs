using Domain.QuoteRequests;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class QuoteAttachmentMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<QuoteAttachment>();

            config.Property(x => x.AttachmentUrl)
                .HasMaxLength(1000);
        }
    }
}
