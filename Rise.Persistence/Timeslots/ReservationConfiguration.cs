// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using Rise.Domain.Boats;
// using Rise.Domain.Timeslots;
// using Rise.Domain.Users;

// namespace Rise.Persistence.Timeslots
// {/// <summary>
//  /// Specific configuration for <see cref="Reservation"/>.
//  /// </summary>
//     internal class ReservationConfiguration : EntityConfiguration<Reservation>
//     {
//         public override void Configure(EntityTypeBuilder<Reservation> builder)
//         {
//             base.Configure(builder);
//             builder.Property(e => e.AmountAdults);
//             builder.Property(e => e.AmountChildren);
//             builder.Property(e => e.AmountPets);

//             builder
//                 .HasOne(e => (Boat)e.Boat)
//                 .WithMany(e => (ICollection<Reservation>)e.Reservations)
//                 .HasForeignKey(e => e.BoatId)
//                 .IsRequired(true);

//             builder
//                 .HasOne(e => (Battery)e.Battery)
//                 .WithMany(e => (ICollection<Reservation>)e.Reservations)
//                 .HasForeignKey(e => e.BatteryId)
//                 .IsRequired(true);

//             builder
//                 .HasOne(e => (TimeSlot)e.TimeSlot)
//                 .WithMany(e => (ICollection<Reservation>)e.Reservations)
//                 .HasForeignKey(e => e.TimeSlotId)
//                 .IsRequired(true);

//             builder
//                 .HasMany(e => (ICollection<User>)e.Users)
//                 .WithMany(e => (ICollection<Reservation>)e.Reservations);
//         }
//     }
// }