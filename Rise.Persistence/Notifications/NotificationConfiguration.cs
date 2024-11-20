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

            builder
                .HasOne(e => (User)e.User)
                .WithMany(e => (ICollection<Notification>)e.Notifications)
                .HasForeignKey(e => e.UserId)
                .IsRequired(true);
        }
    }
}