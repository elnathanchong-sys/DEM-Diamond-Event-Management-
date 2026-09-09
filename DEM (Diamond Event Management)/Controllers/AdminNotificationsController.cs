using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEM__Diamond_Event_Management_.Controllers
{
    public class AdminNotificationsController : Controller
    {
        private readonly DB _db;

        public AdminNotificationsController(DB db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new AdminNotificationDashboardVM();

            // 1. Posted Announcements from the database
            model.Announcements = _db.Announcement
                .OrderByDescending(a => a.PostedAt)
                .Select(a => new AnnouncementItem
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    Type = a.Type,
                    PostedAt = a.PostedAt
                })
                .ToList();

            // 2. Schedule Changes & Event Timings from Event table
            model.ScheduleUpdates = _db.Event
                .Select(e => new ScheduleNotificationItem
                {
                    EventId = e.Id,
                    EventName = e.Name,
                    StartDate = e.StartDate,
                    StartTime = e.StartTime,
                    EndDate = e.EndDate,
                    EndTime = e.EndTime,
                    Status = e.Status
                })
                .ToList();

            // 3. Combined Registration Approvals/Rejections from Registration & TournamentRegistration tables
            var eventRegistrations = _db.Registration
                .Include(r => r.Event)
                .Where(r => r.Status == "Approved" || r.Status == "Denied" || r.Status == "Rejected")
                .Select(r => new RegistrationNotificationItem
                {
                    RegistrationId = r.Id,
                    TeamName = r.TeamName,
                    EventOrTournamentName = r.Event != null ? r.Event.Name : "Event",
                    Status = r.Status,
                    DateInfo = r.RegDate.ToString("dd MMM yyyy")
                })
                .ToList();

            var tournamentRegistrations = _db.TournamentRegistration
                .Include(tr => tr.Tournament)
                .Include(tr => tr.Team)
                .Where(tr => tr.Status != RegistrationStatus.Pending)
                .Select(tr => new RegistrationNotificationItem
                {
                    TeamName = tr.Team != null ? tr.Team.Name : "Team",
                    EventOrTournamentName = tr.Tournament != null ? tr.Tournament.Name : "Tournament",
                    Status = tr.Status.ToString(),
                    AdminNote = tr.AdminNote,
                    DateInfo = tr.ReviewedAt.HasValue ? tr.ReviewedAt.Value.ToString("dd MMM yyyy") : tr.RegisteredAt.ToString("dd MMM yyyy")
                })
                .ToList();

            model.RegistrationLogs = eventRegistrations.Concat(tournamentRegistrations).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult PostAnnouncement(string title, string message, string type)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(message))
            {
                _db.Announcement.Add(new Announcement
                {
                    Title = title,
                    Message = message,
                    Type = type,
                    PostedAt = DateTime.Now
                });

                _db.SaveChanges();

                TempData["Info"] = "Announcement successfully posted!";
            }

            return RedirectToAction("Index");
        }
    }
}