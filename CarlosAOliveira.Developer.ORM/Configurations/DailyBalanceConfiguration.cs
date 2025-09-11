using CarlosAOliveira.Developer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarlosAOliveira.Developer.ORM.Configurations
{
    /// <summary>
    /// Entity Framework configuration for DailyBalance entity
    /// </summary>
    public class DailyBalanceConfiguration : IEntityTypeConfiguration<DailyBalance>
    {
        public void Configure(EntityTypeBuilder<DailyBalance> builder)
        {
            // Table name
            builder.ToTable("DailyBalances");

            // Primary key
            builder.HasKey(db => db.Id);

            // Properties configuration
            builder.Property(db => db.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(db => db.Date)
                .IsRequired()
                .HasConversion<DateOnlyConverter, DateOnlyComparer>()
                .HasColumnName("Date");

            builder.Property(db => db.Balance)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Balance");

            builder.Property(db => db.LastUpdated)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasColumnName("LastUpdated");

            // Indexes
            builder.HasIndex(db => db.Date)
                .IsUnique()
                .HasDatabaseName("IX_DailyBalances_Date");

            builder.HasIndex(db => db.LastUpdated)
                .HasDatabaseName("IX_DailyBalances_LastUpdated");
        }
    }

    /// <summary>
    /// Converter for DateOnly to string for SQLite compatibility
    /// </summary>
    public class DateOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateOnly, string>
    {
        public DateOnlyConverter() : base(
            dateOnly => dateOnly.ToString("yyyy-MM-dd"),
            dateString => DateOnly.Parse(dateString))
        {
        }
    }

    /// <summary>
    /// Comparer for DateOnly
    /// </summary>
    public class DateOnlyComparer : Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<DateOnly>
    {
        public DateOnlyComparer() : base(
            (c1, c2) => c1.Equals(c2),
            c => c.GetHashCode())
        {
        }
    }
}
