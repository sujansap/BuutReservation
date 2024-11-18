namespace Rise.Shared.Notifications
{
    public class NotificationDto : BaseDto
    {
        public string Severity { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
