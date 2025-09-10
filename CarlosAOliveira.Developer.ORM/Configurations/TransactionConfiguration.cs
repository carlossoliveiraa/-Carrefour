using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarlosAOliveira.Developer.ORM.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Transaction entity
    /// </summary>
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // Table name
            builder.ToTable("Transactions");

            // Primary key
            builder.HasKey(t => t.Id);

            // Properties configuration
            builder.Property(t => t.Id)
                .IsRequired()
                .ValueGeneratedNever(); // We generate GUIDs manually

            builder.Property(t => t.MerchantId)
                .IsRequired()
                .HasColumnName("MerchantId");

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Amount");

            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("Type");

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("Description");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasColumnName("CreatedAt");

            // Indexes
            builder.HasIndex(t => t.MerchantId)
                .HasDatabaseName("IX_Transactions_MerchantId");

            builder.HasIndex(t => t.CreatedAt)
                .HasDatabaseName("IX_Transactions_CreatedAt");

            builder.HasIndex(t => new { t.MerchantId, t.CreatedAt })
                .HasDatabaseName("IX_Transactions_MerchantId_CreatedAt");

            builder.HasIndex(t => t.Type)
                .HasDatabaseName("IX_Transactions_Type");

            // Foreign key relationship
            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(t => t.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
