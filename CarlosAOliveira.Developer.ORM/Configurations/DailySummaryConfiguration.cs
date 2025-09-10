using CarlosAOliveira.Developer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarlosAOliveira.Developer.ORM.Configurations
{
    /// <summary>
    /// Entity Framework configuration for DailySummary entity
    /// </summary>
    public class DailySummaryConfiguration : IEntityTypeConfiguration<DailySummary>
    {
        public void Configure(EntityTypeBuilder<DailySummary> builder)
        {
            // Table name
            builder.ToTable("DailySummaries");

            // Primary key
            builder.HasKey(ds => ds.Id);

            // Properties configuration
            builder.Property(ds => ds.Id)
                .IsRequired()
                .ValueGeneratedNever(); // We generate GUIDs manually

            builder.Property(ds => ds.MerchantId)
                .IsRequired()
                .HasColumnName("MerchantId");

            builder.Property(ds => ds.Date)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("Date");

            builder.Property(ds => ds.TotalCredits)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("TotalCredits");

            builder.Property(ds => ds.TotalDebits)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("TotalDebits");

            builder.Property(ds => ds.NetAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("NetAmount");

            builder.Property(ds => ds.TransactionCount)
                .IsRequired()
                .HasColumnName("TransactionCount");

            // Indexes
            builder.HasIndex(ds => ds.MerchantId)
                .HasDatabaseName("IX_DailySummaries_MerchantId");

            builder.HasIndex(ds => ds.Date)
                .HasDatabaseName("IX_DailySummaries_Date");

            builder.HasIndex(ds => new { ds.MerchantId, ds.Date })
                .IsUnique()
                .HasDatabaseName("IX_DailySummaries_MerchantId_Date");

            // Foreign key relationship
            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(ds => ds.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
