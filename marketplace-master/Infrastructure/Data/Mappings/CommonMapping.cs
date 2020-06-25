using Domain.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Infrastructure.Data.Mappings
{
    internal class CommonMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("varchar"));

            modelBuilder.Properties<string>()
                .Where(x => x.CustomAttributes.Any(a => typeof(TextAttribute).IsAssignableFrom(a.AttributeType)))
                .Configure(x => x.HasColumnType("nvarchar(max)"));
        }
    }
}