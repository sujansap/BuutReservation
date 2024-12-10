namespace Rise.Shared.Notifications;

public interface IInternalNotificationService
{
    Task SendNotificationToUser(int userId, string title, string message, SeverityEnum severity);

}
