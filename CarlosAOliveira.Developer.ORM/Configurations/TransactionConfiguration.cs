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

            builder.Property(t => t.Date)
                .IsRequired()
                .HasConversion<DateOnlyConverter, DateOnlyComparer>()
                .HasColumnName("Date");

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Amount");

            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("Type");

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Category");

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("Description");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasColumnName("CreatedAt");

            // Indexes
            builder.HasIndex(t => t.Date)
                .HasDatabaseName("IX_Transactions_Date");

            builder.HasIndex(t => t.CreatedAt)
                .HasDatabaseName("IX_Transactions_CreatedAt");

            builder.HasIndex(t => t.Type)
                .HasDatabaseName("IX_Transactions_Type");

            builder.HasIndex(t => t.Category)
                .HasDatabaseName("IX_Transactions_Category");
        }
    }
}

