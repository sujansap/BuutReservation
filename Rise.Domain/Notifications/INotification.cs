using Rise.Domain.Users;

namespace Rise.Domain.Notifications
{
    /// <summary>
    /// Represents a notification that can be sent to a user.
    /// </summary>
    public interface INotification : IEntity
    {
        public int Severity { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int UserId { get; }
        public IUser User { get; }
    }
}