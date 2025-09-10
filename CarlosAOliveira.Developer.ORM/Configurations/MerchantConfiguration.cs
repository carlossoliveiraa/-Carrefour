using CarlosAOliveira.Developer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarlosAOliveira.Developer.ORM.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Merchant entity
    /// </summary>
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            // Table name
            builder.ToTable("Merchants");

            // Primary key
            builder.HasKey(m => m.Id);

            // Properties configuration
            builder.Property(m => m.Id)
                .IsRequired()
                .ValueGeneratedNever(); // We generate GUIDs manually

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("Name");

            builder.Property(m => m.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Email");

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasColumnName("CreatedAt");

            // Indexes
            builder.HasIndex(m => m.Email)
                .IsUnique()
                .HasDatabaseName("IX_Merchants_Email");

            builder.HasIndex(m => m.CreatedAt)
                .HasDatabaseName("IX_Merchants_CreatedAt");

            // Relationships
            builder.HasMany<Transaction>()
                .WithOne()
                .HasForeignKey(t => t.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<DailySummary>()
                .WithOne()
                .HasForeignKey(ds => ds.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
