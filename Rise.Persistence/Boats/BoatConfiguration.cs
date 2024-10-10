// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using Rise.Domain.Boats;

// namespace Rise.Persistence.Boats
// {
//     /// <summary>
//     /// Specific configuration for <see cref="Boat"/>.
//     /// </summary>
//     internal class BoatConfiguration : EntityConfiguration<Boat>
//     {
//         public override void Configure(EntityTypeBuilder<Boat> builder)
//         {
//             base.Configure(builder);
//             builder.Property(x => x.PersonalName).HasMaxLength(64);
//             builder.Property(x => x.StartOutOfOrder).HasDefaultValue(null);
//             builder.Property(x => x.EndOutOfOrder).HasDefaultValue(null);
//             builder.Property(x => x.MaximumAdults).HasDefaultValue(1);
//             builder.Property(x => x.MaximumChildren).HasDefaultValue(0);
//             builder.Property(x => x.MaximumPets).HasDefaultValue(0);
//         }
//     }
// }