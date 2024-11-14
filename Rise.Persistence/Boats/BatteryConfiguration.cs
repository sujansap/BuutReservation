using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Boats;
using Rise.Domain.Users;

namespace Rise.Persistence.Boats
{
    /// <summary>
    /// Specific configuration for <see cref="Battery"/>.
    /// </summary>
    internal class BatteryConfiguration : EntityConfiguration<Battery>
    {
        public override void Configure(EntityTypeBuilder<Battery> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Type).HasMaxLength(64);
            builder.Property(x => x.BoatId);

            builder.HasOne(x => (Boat) x.Boat)
            .WithMany(b => b.Batteries)
            .HasForeignKey(b => b.BoatId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => (User)x.Mentor)
            .WithOne()
            .HasForeignKey<Battery>(x => x.MentorId)
            .OnDelete(DeleteBehavior.SetNull);

            // TODO:
            // Boat connectie maken met battey, domeintesten van battery, meterEnPeterRelatie doen
            // voor hulp kijk in de code en naar de entity framework docs
        }
    }
}