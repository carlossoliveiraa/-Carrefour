using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanCode.ORM.Configurations
{
    /// <summary>
    /// Configuração do Entity Framework para a entidade Transaction
    /// </summary>
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        /// <summary>
        /// Configura a entidade Transaction
        /// </summary>
        /// <param name="builder">Builder da entidade</param>
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // Tabela
            builder.ToTable("Transactions");

            // Chave primária
            builder.HasKey(t => t.Id);

            // Propriedades
            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            builder.Property(t => t.Category)
                .HasMaxLength(100);

            builder.Property(t => t.Notes)
                .HasMaxLength(500);

            // Índices
            builder.HasIndex(t => t.TransactionDate)
                .HasDatabaseName("IX_Transactions_TransactionDate");

            builder.HasIndex(t => t.Type)
                .HasDatabaseName("IX_Transactions_Type");

            builder.HasIndex(t => t.Category)
                .HasDatabaseName("IX_Transactions_Category");

            builder.HasIndex(t => new { t.TransactionDate, t.Type })
                .HasDatabaseName("IX_Transactions_Date_Type");

            // Valores padrão
            builder.Property(t => t.TransactionDate)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
