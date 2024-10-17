// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using Rise.Domain.Boats;

// namespace Rise.Persistence.Boats
// {
//     /// <summary>
//     /// Specific configuration for <see cref="Battery"/>.
//     /// </summary>
//     internal class BatteryConfiguration : EntityConfiguration<Battery>
//     {
//         public override void Configure(EntityTypeBuilder<Battery> builder)
//         {
//             base.Configure(builder);
//             builder.Property(x => x.Type).HasMaxLength(64);
//             builder.Property(x => x.MaximumCapacity);
//             builder.Property(x => x.CurrentLoad);
//             builder.Property(x => x.OutOfOrder);
//             builder.Property(x => x.BoatId);

//             builder
//                 .HasOne(e => (Boat)e.Boat)
//                 .WithMany(e => (ICollection<Battery>)e.Batteries)
//                 .HasForeignKey(e => e.BoatId)
//                 .IsRequired(false);
//         }
//     }
// }