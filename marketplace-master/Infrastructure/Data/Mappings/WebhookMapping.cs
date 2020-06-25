using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class WebhookMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<Webhook>();

            config.HasRequired(x => x.ApiToken)
                .WithMany()
                .HasForeignKey(x => x.ApiToken_Id);
        }
    }
}