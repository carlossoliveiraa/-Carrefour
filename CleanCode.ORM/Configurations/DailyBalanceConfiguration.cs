using CleanCode.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanCode.ORM.Configurations
{
    /// <summary>
    /// Configuração do Entity Framework para a entidade DailyBalance
    /// </summary>
    public class DailyBalanceConfiguration : IEntityTypeConfiguration<DailyBalance>
    {
        /// <summary>
        /// Configura a entidade DailyBalance
        /// </summary>
        /// <param name="builder">Builder da entidade</param>
        public void Configure(EntityTypeBuilder<DailyBalance> builder)
        {
            // Tabela
            builder.ToTable("DailyBalances");

            // Chave primária
            builder.HasKey(db => db.Id);

            // Propriedades
            builder.Property(db => db.Date)
                .IsRequired();

            builder.Property(db => db.OpeningBalance)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(db => db.TotalCredits)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(db => db.TotalDebits)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(db => db.ClosingBalance)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(db => db.CreditTransactionCount)
                .IsRequired();

            builder.Property(db => db.DebitTransactionCount)
                .IsRequired();

            builder.Property(db => db.TotalTransactionCount)
                .IsRequired();

            builder.Property(db => db.LastUpdated)
                .IsRequired();

            // Índices
            builder.HasIndex(db => db.Date)
                .IsUnique()
                .HasDatabaseName("IX_DailyBalances_Date");

            builder.HasIndex(db => db.Date)
                .HasDatabaseName("IX_DailyBalances_Date_NonUnique");

            // Valores padrão
            builder.Property(db => db.OpeningBalance)
                .HasDefaultValue(0);

            builder.Property(db => db.TotalCredits)
                .HasDefaultValue(0);

            builder.Property(db => db.TotalDebits)
                .HasDefaultValue(0);

            builder.Property(db => db.ClosingBalance)
                .HasDefaultValue(0);

            builder.Property(db => db.CreditTransactionCount)
                .HasDefaultValue(0);

            builder.Property(db => db.DebitTransactionCount)
                .HasDefaultValue(0);

            builder.Property(db => db.TotalTransactionCount)
                .HasDefaultValue(0);

            builder.Property(db => db.LastUpdated)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
