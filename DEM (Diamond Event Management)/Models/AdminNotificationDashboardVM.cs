using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models
{
    public class AdminNotificationDashboardVM
    {
        public List<AnnouncementItem> Announcements { get; set; } = new();
        public List<ScheduleNotificationItem> ScheduleUpdates { get; set; } = new();
        public List<RegistrationNotificationItem> RegistrationLogs { get; set; } = new();
    }

    public class AnnouncementItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "General";
        public DateTime PostedAt { get; set; } = DateTime.Now;
    }

    public class ScheduleNotificationItem
    {
        public string EventId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class RegistrationNotificationItem
    {
        public string RegistrationId { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string EventOrTournamentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AdminNote { get; set; }
        public string DateInfo { get; set; } = string.Empty;
    }
}