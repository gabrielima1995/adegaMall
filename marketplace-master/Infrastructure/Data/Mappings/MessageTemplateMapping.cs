using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class MessageTemplateMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<MessageTemplate>();

            config.Property(x => x.Key)
                .HasMaxLength(100);

            config.Property(x => x.Lang)
                .HasMaxLength(7);

            config.Property(x => x.SubjectTemplate)
                .HasMaxLength(256);
        }
    }
}