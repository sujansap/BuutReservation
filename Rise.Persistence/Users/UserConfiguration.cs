using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Users
{
    /// <summary>
    /// Specific configuration for <see cref="User"/>.
    /// </summary>
    internal class UserConfiguration: EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.FamilyName).HasMaxLength(65);
        }
    }
}