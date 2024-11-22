using System;
using Rise.Domain.Users;

namespace Rise.Domain.Notifications
{
    public class Notification : Entity
    {
        public int Severity { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public required bool IsRead { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}


