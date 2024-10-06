using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Timeslots;

internal class CruisePeriodConfiguration : IEntityTypeConfiguration<CruisePeriod>
{
    public void Configure(EntityTypeBuilder<CruisePeriod> builder)
    {
        builder.Property(x => x.Start).IsRequired();
        builder.Property(x => x.End).IsRequired();

        // Configure relationships if needed
        builder.HasMany(x => x.TimeSlots)
               .WithOne(x => x.CruisePeriod)
               .HasForeignKey(x => x.CruisePeriodId);
    }
}
