using Domain.Modules.Finance;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class TransferJournalEntryMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<TransferJournalEntry>();

            config.ToTable("TransferJournal");

            config.Property(x => x.BuyerId)
                .HasMaxLength(128);

            config.Property(x => x.SellerId)
                .HasMaxLength(128);

            config.Property(x => x.PaymentMethod)
                .HasMaxLength(30);

            config.Property(x => x.OrderDate)
                .HasColumnType("datetime2");

            config.Property(x => x.OrderTotal)
                .HasPrecision(7, 2);

            config.Property(x => x.CommissionPct)
                .HasPrecision(3, 2);

            config.Property(x => x.InstallmentCommissionAmount)
                .HasPrecision(7, 2);

            config.Property(x => x.InstallmentAmount)
                .HasPrecision(7, 2);

            config.Property(x => x.TransferAmount)
                .HasPrecision(7, 2);

            config.Property(x => x.DueDate)
                .HasColumnType("datetime2");

            config.Property(x => x.CompletedDate)
                .HasColumnType("datetime2");

            config.Property(x => x.GatewayReceivableId)
                .HasMaxLength(50);

            config.Property(x => x.GatewayReceiverId)
                .HasMaxLength(50);

            config.Property(x => x.GatewayTransactionId)
                .HasMaxLength(50);

            config.Property(x => x.CancelledDate)
                .HasColumnType("datetime2");

            config.Property(x => x.AdditionalNotes)
                .HasColumnType("nvarchar(max)");

            config.Property(x => x.CreatedAt)
                .HasColumnType("datetime2");

            config.Ignore(x => x.KeyForGatewayComparison);
        }
    }
}
