using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Notifications;
using Rise.Domain.Users;

namespace Rise.Persistence.Notifications
{
    internal class NotificationConfiguration : EntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Title)
                        .HasMaxLength(130);

            builder
                .HasOne(e => e.User)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.UserId)
                .IsRequired(true);
        }
    }
}