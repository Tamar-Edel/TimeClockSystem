using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClockSystem.Core.Entities;

namespace TimeClockSystem.Infrastructure.Persistence.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.ClockInAt)
            .IsRequired();

        builder.Property(s => s.ClockOutAt)
            .IsRequired(false);

        builder.Property(s => s.ClockInTimeSource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.ClockOutTimeSource)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(s => s.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId);

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasFilter("[ClockOutAt] IS NULL")
            .HasDatabaseName("IX_Shifts_UserId_OpenShift");
    }
}
