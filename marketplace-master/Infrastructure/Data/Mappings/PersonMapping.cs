using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class PersonMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var person = modelBuilder.Entity<Person>().ToTable("Person");

            person.Property(p => p.UtmTags)
                .HasMaxLength(1500);

            person.Property(p => p.ModifiedAt)
                .HasColumnType("datetime2");

            person.Property(p => p.UserId)
                .HasMaxLength(128)
                .IsRequired();
        }
    }
}
