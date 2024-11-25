namespace Rise.Shared.Notifications
{

    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUserNotifications();
        Task MarkNotificationAsRead(int id);
    }

}
