using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarlosAOliveira.Developer.ORM.Configurations
{
    /// <summary>
    /// Entity Framework configuration for User entity
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table name
            builder.ToTable("Users");

            // Primary key
            builder.HasKey(u => u.Id);

            // Properties configuration
            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedNever(); // We generate GUIDs manually

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("Username");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Email");

            builder.Property(u => u.Phone)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("Phone");

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Password");

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("Role");

            builder.Property(u => u.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("Status");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasColumnName("CreatedAt");

            builder.Property(u => u.UpdatedAt)
                .HasColumnType("datetime2")
                .HasColumnName("UpdatedAt");

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("IX_Users_Username");

            builder.HasIndex(u => u.CreatedAt)
                .HasDatabaseName("IX_Users_CreatedAt");

            builder.HasIndex(u => u.Role)
                .HasDatabaseName("IX_Users_Role");

            builder.HasIndex(u => u.Status)
                .HasDatabaseName("IX_Users_Status");
        }
    }
}
