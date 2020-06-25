using Domain.Modules.Customization.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class PlatformCustomizationMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<PlatformCustomization>();

            config.Property(x => x.HtmlHead)
                .HasColumnType("nvarchar(max)");

            config.Property(x => x.HtmlBodyStart)
                .HasColumnType("nvarchar(max)");

            config.Property(x => x.HtmlBodyEnd)
                .HasColumnType("nvarchar(max)");
        }
    }
}
