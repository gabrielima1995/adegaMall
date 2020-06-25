using Microsoft.EntityFrameworkCore;
using NewContext.Domain.Languages;
using System.Linq;

namespace NewContext.Infrastructure.Data.Mappers
{
    internal class PlatformLanguagesMapper : IMapper
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<PlatformLanguage>().ToTable("PlatformLanguage", DefaultSchema.Value);

            entity.Property<int>("Id").HasDefaultValue(0);
            entity.HasKey("Id");

            entity.Property(e => e.ActiveLanguages)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasMaxLength(100)
                .IsRequired()
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',').Select(lang => (Language)lang).ToList());

            entity.Property(e => e.DefaultLanguage)
                .HasMaxLength(10)
                .IsRequired()
                .HasConversion(
                    v => v.Specifier,
                    v => (Language)v);
        }
    }
}
