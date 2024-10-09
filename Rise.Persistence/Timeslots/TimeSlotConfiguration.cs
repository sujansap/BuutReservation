using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Timeslots;

internal class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Start).IsRequired();
        builder.Property(x => x.End).IsRequired();

        // Configure foreign key relationship
        builder.HasOne(x => (CruisePeriod)x.CruisePeriod)
               .WithMany(x => (ICollection<TimeSlot>)x.TimeSlots)
               .HasForeignKey(x => x.CruisePeriodId)
               .OnDelete(DeleteBehavior.Cascade);
        //    TODO verify onDelete for TimeSlotConfiguration
    }
}
