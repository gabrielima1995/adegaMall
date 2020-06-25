using Domain.Modules.Finance;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class FinanceMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            MapGatewayRegistrationLog(modelBuilder);
        }

        private void MapGatewayRegistrationLog(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GatewayRegistrationLog>();

            entity.ToTable("GatewayRegistrationLog");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SellerId).HasMaxLength(36);
            entity.Property(e => e.Gateway).HasMaxLength(10);
            entity.Property(e => e.GatewayRecipientId).HasMaxLength(50);
            entity.Property(e => e.GatewayBankAccountId).HasMaxLength(50);
            entity.Property(e => e.GatewayApiToken).HasMaxLength(50);
            entity.Property(e => e.ExtraInfo);

            entity.Ignore(e => e.IsPagarme);
            entity.Ignore(e => e.IsWirecard);
        }
    }
}
